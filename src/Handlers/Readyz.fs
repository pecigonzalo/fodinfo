[<AutoOpen>]
module fodinfo.Handlers.Readyz

open Falco
open fodinfo.HealthChecks

// `/readyz` is handled by .NET HealthChecks

let handleReadyzEnable : HttpHandler =
    fun ctx ->
        Toggle.Agent.Enable()
        Response.withStatusCode 202 >> Response.ofEmpty <| ctx

let handleReadyzDisable : HttpHandler =
    fun ctx ->
        Toggle.Agent.Disable()
        Response.withStatusCode 202 >> Response.ofEmpty <| ctx
