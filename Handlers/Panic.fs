module fodinfo.Handlers.Panic

open Falco

let handlePanic : HttpHandler =
    fun _ -> failwith "Panic command received"
