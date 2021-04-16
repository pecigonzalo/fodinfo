module fodinfo.Handlers.Echo

open Falco

let handleEcho : HttpHandler = Response.ofJson "OK"
