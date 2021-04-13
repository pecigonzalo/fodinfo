module fodinfo.Routing

open Giraffe
open Microsoft.AspNetCore.Http
open fodinfo.Handlers

let api : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ GET >=> route "/hello" >=> handleGetHello
             GET >=> route "/info" >=> handleInfo ]

let routes : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ route "/healthz" >=> handleHealthz
             subRoute "/api" api
             setStatusCode 404 >=> text "Not Found" ]
