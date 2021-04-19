module fodinfo.Handlers.Env

open Falco
open System

let handleEnv : HttpHandler =
    Environment.GetEnvironmentVariables()
    |> Response.ofJson
