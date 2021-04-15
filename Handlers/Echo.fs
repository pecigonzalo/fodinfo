module fodinfo.Handlers.Echo

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe

let handleEcho : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let response = "OK"
            return! json response next ctx
        }
