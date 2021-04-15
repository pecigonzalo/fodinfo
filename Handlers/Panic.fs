module fodinfo.Handlers.Panic

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe

let handlePanic : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) -> failwith "Panic command received"
