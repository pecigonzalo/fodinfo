[<AutoOpen>]
module fodinfo.Handlers.Panic

open Falco
open Serilog
open System
open System.Threading.Tasks

let handlePanic : HttpHandler =
    fun _ ->
        Log.Information("Panic command received")
        Log.CloseAndFlush()
        Environment.Exit(255)
        Task.CompletedTask
