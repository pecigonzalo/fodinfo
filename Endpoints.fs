module fodinfo.Endpoints

open fodinfo.Handlers
open Giraffe.EndpointRouting

let apiEndpoints =
    [ GET [ route "/info" Info.handleInfo
            route "/panic" Panic.handlePanic
            route "/echo" Echo.handleEcho ] ]

let endpoints =
    [ GET [ route "/healthz" Healthz.handleHealthz
            route "/version" Version.handleVersion
            subRoute "/api" apiEndpoints ] ]
