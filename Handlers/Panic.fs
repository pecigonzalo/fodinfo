module fodinfo.Handlers.Panic

open FSharp.Control.Tasks
open Falco

let handlePanic : HttpHandler =
    fun _ -> task { failwith "Panic command received" }
