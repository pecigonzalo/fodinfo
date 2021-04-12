module podinfo.Routing

open Giraffe
open Microsoft.AspNetCore.Http
open podinfo.Handlers

let api : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ GET
             >=> choose [ route "/hello" >=> handleGetHello ] ]

let routes : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ route "/healthz" >=> handleHealthz
             subRoute "/api" api
             setStatusCode 404 >=> text "Not Found" ]
