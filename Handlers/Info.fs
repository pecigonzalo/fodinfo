module fodinfo.Handlers.Info

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe
open fodinfo

type RunetimeResponse =
    { Hostname: string
      Version: string
      Revision: string
      Color: string
      Logo: string
      Message: string
      OS: string
      ARCH: string
      Runtime: string
      NumCPU: string }

let handleInfo : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let config = Config.getConfiguration

            let response =
                { Hostname = config.hostname
                  Version = config.version
                  Revision = "OK"
                  Color = config.uiColor
                  Logo = config.uiLogo
                  Message = config.uiMessage
                  OS = config.os
                  ARCH = config.arch
                  Runtime = config.runtime
                  NumCPU = config.num_cpu }

            return! json response next ctx
        }
