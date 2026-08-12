[<AutoOpen>]
module fodinfo.Handlers.Delay

open Falco
open System
open System.Threading.Tasks

let handleDelay : HttpHandler =
    fun ctx ->
        task {
            let route = Request.getRoute ctx

            match route.TryGetInt "seconds" with
            | None ->
                return! Response.withStatusCode 400 >> Response.ofPlainText "invalid seconds" <| ctx
            | Some seconds ->
                let delay = TimeSpan.FromSeconds(float seconds)

                do! Task.Delay(max TimeSpan.Zero delay)
                return! Response.ofJson {| seconds = seconds |} <| ctx
        }
