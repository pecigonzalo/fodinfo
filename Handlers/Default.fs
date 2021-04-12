module podinfo.Handlers

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe

let handleGetHello =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let response = "Hello world, from Giraffe!"
            return! json response next ctx
        }

let handleHealthz =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let response = "OK"
            return! json response next ctx
        }
