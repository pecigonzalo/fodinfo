module fodinfo.Handlers.Healthz

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe

let handleHealthz : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let response = "OK"
            return! json response next ctx
        }
