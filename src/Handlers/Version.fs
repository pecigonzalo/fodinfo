[<AutoOpen>]
module fodinfo.Handlers.Version

open Falco

let handleVersion : HttpHandler =
    {| commit = fodinfo.Config.Revision
       version = fodinfo.Config.Runtime.version |}
    |> Response.ofJson
