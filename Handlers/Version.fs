module fodinfo.Handlers.Version

open Falco

let handleVersion : HttpHandler =
    {| Version = fodinfo.Config.Runtime.version |}
    |> Response.ofJson
