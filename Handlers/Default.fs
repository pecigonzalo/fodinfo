module fodinfo.Handlers

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Routing
open FSharp.Control.Tasks
open Giraffe
open fodinfo

let handleHealthz : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let response = "OK"
            return! json response next ctx
        }

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

let handleEcho : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let response = "OK"
            return! json response next ctx
        }

let handlePanic : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) -> failwith "Panic command received"

let handleVersion : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        // ctx.Request.Path
        let a = ctx.GetRouteData()
        printfn "%A %A" a.Values a.DataTokens
        let b = ctx.GetEndpoint()
        printfn "%A" b

        task {
            let config = Config.getConfiguration
            let response = config.version
            return! json response next ctx
        }
