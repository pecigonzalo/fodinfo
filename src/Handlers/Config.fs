[<AutoOpen>]
module fodinfo.Handlers.Config

open Falco
open Microsoft.Extensions.Configuration

let handleConfig : HttpHandler =
    fun ctx ->
        let config = ctx.Plug<IConfiguration>() :?> IConfigurationRoot

        config.GetDebugView() |> Response.ofPlainText
        <| ctx
