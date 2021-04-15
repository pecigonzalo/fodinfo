module fodinfo.Handlers.Version

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe
open fodinfo

let handleVersion : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let config = Config.getConfiguration
            let response = config.version
            return! json response next ctx
        }
