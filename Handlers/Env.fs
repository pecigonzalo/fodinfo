module fodinfo.Handlers.Env

open System
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe

let handleEnv : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let envVars = Environment.GetEnvironmentVariables()

            return! json envVars next ctx
        }
