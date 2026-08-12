[<AutoOpen>]
module fodinfo.Handlers.Store

open Falco
open Microsoft.Extensions.Options
open Serilog
open System.IO
open System.Text
open System.Text.RegularExpressions
open System.Security.Cryptography

let private validHash = Regex("^[a-f0-9]{40}$")

let handleStoreGet : HttpHandler =
    fun ctx ->
        let route = Request.getRoute ctx

        let config = ctx.Plug<IOptionsSnapshot<fodinfo.Config.Configuration>>().Value

        match route.TryGetString "hash" with
        | None ->
            Response.withStatusCode 404
            >> Response.ofPlainText "No hash found in route"
            <| ctx
        | Some hash when validHash.IsMatch hash ->
            let path = Path.Combine(config.DataPath, hash)

            try
                File.ReadAllText path |> Response.ofPlainText <| ctx
            with :? FileNotFoundException ->
                Response.withStatusCode 404
                >> Response.ofPlainText "file not found"
                <| ctx
        | Some _ ->
            Response.withStatusCode 400
            >> Response.ofPlainText "invalid hash"
            <| ctx

let stringToSha1 (string: string) =
    use sha1 = SHA1.Create()

    string
    |> Encoding.UTF8.GetBytes
    |> sha1.ComputeHash
    |> Seq.map (fun c -> c.ToString("x2"))
    |> Seq.reduce (+)

let handleStorePost : HttpHandler =
    fun ctx ->
        task {
            let config = ctx.Plug<IOptionsSnapshot<fodinfo.Config.Configuration>>().Value

            let! body = Request.getBodyString ctx

            let hash = body |> stringToSha1
            let path = Path.Combine(config.DataPath, hash)

            try
                do! File.WriteAllTextAsync(path, body)
                return! {| hash = hash |} |> Response.ofJson <| ctx
            with ex ->
                Log.Warning(ex, "writing file failed")
                return! Response.withStatusCode 500 >> Response.ofPlainText "writing file failed" <| ctx
        }
