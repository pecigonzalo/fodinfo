[<AutoOpen>]
module fodinfo.Handlers.Status

open Falco

let handleStatus : HttpHandler =
    fun ctx ->
        let route = Request.getRoute ctx

        match route.TryGetInt "code" with
        | None ->
            Response.withStatusCode 400
            >> Response.ofPlainText "invalid status code"
            <| ctx
        | Some code ->
            Response.withStatusCode code
            >> Response.ofJson {| status = code |}
            <| ctx
