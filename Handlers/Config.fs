module fodinfo.Handlers.Config

open Microsoft.Extensions.Configuration
open Falco

let handleConfig : HttpHandler =
    fun ctx ->
        let config =
            ctx.GetService<IConfiguration>() :?> IConfigurationRoot

        config.GetDebugView() |> Response.ofPlainText
        <| ctx
