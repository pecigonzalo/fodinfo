module fodinfo.Handlers.Headers

open Falco

let handleHeaders : HttpHandler =
    fun ctx -> Response.ofJson ctx.Request.Headers ctx
