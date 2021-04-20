[<AutoOpen>]
module fodinfo.Handlers.Echo

open Falco
open FSharp.Control.Tasks
open Microsoft.Extensions.Options
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open System.Net.Http
open System.Collections.Generic


let addHeader (header: KeyValuePair<string, StringValues>) (request: HttpRequestMessage) =
    request.Headers.Add(header.Key, header.Value)
    request

let addContent (content: string) (request: HttpRequestMessage) =
    request.Content <- new StringContent(content)
    request

let makeRequest (client: HttpClient) (request: HttpRequestMessage) = client.SendAsync(request)

let copyTracingHeader (headers: IHeaderDictionary) (request: HttpRequestMessage) =
    let tracingHeaders =
        [ "x-request-id"
          "x-b3-traceid"
          "x-b3-spanid"
          "x-b3-parentspanid"
          "x-b3-sampled"
          "x-b3-flags"
          "x-ot-span-context" ]

    let tracingHeaderFolder (state: HttpRequestMessage) (header: KeyValuePair<string, StringValues>) =
        match List.contains header.Key tracingHeaders with
        | true -> state |> addHeader header
        | false -> state

    headers |> Seq.fold (tracingHeaderFolder) request

// TODO: There must be a more idiomatic way of doing this entire handler
let handleEcho : HttpHandler =
    fun ctx ->
        task {
            let config =
                ctx
                    .GetService<IOptionsSnapshot<fodinfo.Config.Configuration>>()
                    .Value

            let client =
                ctx
                    .GetService<IHttpClientFactory>()
                    .CreateClient()

            let! body = ctx.Request.GetBodyAsync()

            use httpRequestMessage =
                new HttpRequestMessage(HttpMethod.Post, config.BackendURL)

            try
                let! response =
                    httpRequestMessage
                    |> addContent body
                    |> copyTracingHeader ctx.Request.Headers
                    |> makeRequest client

                let! body = response.Content.ReadAsStringAsync()

                return!
                    Response.withStatusCode (int response.StatusCode)
                    >> Response.ofPlainText body
                    <| ctx
            with :? HttpRequestException as ex ->
                return!
                    Response.withStatusCode 503
                    >> Response.ofPlainText $"Failed to forward request: {ex.Message}"
                    <| ctx
        }
