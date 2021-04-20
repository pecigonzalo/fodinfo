module fodinfo.Handlers.Version

open Falco

let handleVersion : HttpHandler =
    {| commit = ""
       version = fodinfo.Config.Runtime.version |}
    |> Response.ofJson
