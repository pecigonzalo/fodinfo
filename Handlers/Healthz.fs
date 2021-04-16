module fodinfo.Handlers.Healthz

open Falco

let handleHealthz : HttpHandler = Response.ofJson "OK"
