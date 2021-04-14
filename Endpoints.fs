module fodinfo.Endpoints

open fodinfo.Handlers
open Giraffe.EndpointRouting

let apiEndpoints =
    [ GET [ route "/info" handleInfo
            route "/panic" handlePanic
            route "/echo" handleEcho ] ]

let endpoints =
    [ GET [ route "/healthz" handleHealthz
            route "/version" handleVersion
            subRoute "/api" apiEndpoints ] ]
