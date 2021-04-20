[<AutoOpen>]
module fodinfo.Handlers.Delay

open Falco

let handleDelay : HttpHandler =
    fun ctx ->
        let secondsToMs seconds = seconds * 1000

        let route = Request.getRoute ctx

        let seconds = route.GetInt "seconds" 0

        seconds
        |> secondsToMs
        |> Async.Sleep
        |> Async.RunSynchronously

        Response.ofJson seconds ctx
