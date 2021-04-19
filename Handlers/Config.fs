module fodinfo.Handlers.Config

open Falco
open Microsoft.Extensions.Configuration

let handleConfig : HttpHandler =
    fun ctx ->
        let config =
            ctx.GetService<IConfiguration>() :?> IConfigurationRoot

        config.GetDebugView() |> Response.ofPlainText
        <| ctx
