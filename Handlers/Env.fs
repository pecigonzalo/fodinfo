module fodinfo.Handlers.Env

open System
open Falco

let handleEnv : HttpHandler =
    Environment.GetEnvironmentVariables()
    |> Response.ofJson
