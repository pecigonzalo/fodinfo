module fodinfo.Handlers.Info

open Microsoft.Extensions.Options
open Falco

let handleInfo : HttpHandler =
    fun ctx ->
        let config =
            ctx
                .GetService<IOptionsSnapshot<fodinfo.Config.Configuration>>()
                .Value

        let runtime = fodinfo.Config.Runtime

        {| Hostname = runtime.hostname
           Version = runtime.version
           Color = config.UIColor
           Logo = config.UILogo
           Message = config.UIMessage
           Revision = "OK"
           OS = runtime.os
           ARCH = runtime.arch
           Runtime = runtime.runtime
           NumCPU = runtime.num_cpu |}
        |> Response.ofJson
        <| ctx
