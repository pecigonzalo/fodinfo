[<AutoOpen>]
module fodinfo.Handlers.Store

open Falco
open FSharp.Control.Tasks
open Microsoft.Extensions.Options
open System.IO
open System.Text
open System.Security.Cryptography

let handleStoreGet : HttpHandler =
    fun ctx ->
        let route = Request.getRoute ctx

        let config =
            ctx
                .GetService<IOptionsSnapshot<fodinfo.Config.Configuration>>()
                .Value

        match route.TryGetString "hash" with
        | None ->
            Response.withStatusCode 404
            >> Response.ofPlainText "No hash found in route"
            <| ctx
        | Some hash ->
            let path = $"{config.DataPath}/{hash}"

            File.ReadAllText path |> Response.ofPlainText
            <| ctx

let stringToSha1 (string: string) =
    use sha1 = SHA1.Create()

    string
    |> Encoding.ASCII.GetBytes
    |> sha1.ComputeHash
    |> Seq.map (fun c -> c.ToString("X2"))
    |> Seq.reduce (+)

let handleStorePost : HttpHandler =
    fun ctx ->
        task {
            let config =
                ctx
                    .GetService<IOptionsSnapshot<fodinfo.Config.Configuration>>()
                    .Value

            let! body = ctx.Request.GetBodyAsync()

            let hash = body |> stringToSha1

            use writter =
                sprintf "%s/%s" config.DataPath hash
                |> File.CreateText

            body |> writter.Write

            return! {| hash = hash |} |> Response.ofJson <| ctx
        }
