[<AutoOpen>]
module fodinfo.Handlers.Info

open fodinfo
open Falco

let handleInfo : HttpHandler =
    fun ctx ->
        let config = ctx.GetService<Config.Configuration>()

        let runtime = Config.Runtime

        {| hostname = runtime.hostname
           version = runtime.version
           color = config.UIColor
           logo = config.UILogo
           message = config.UIMessage
           revision = "OK"
           os = runtime.os
           arch = runtime.arch
           runtime = runtime.runtime
           numcpu = runtime.num_cpu |}
        |> Response.ofJson
        <| ctx
