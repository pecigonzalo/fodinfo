module fodinfo.Routing

open Giraffe
open Microsoft.AspNetCore.Http
open fodinfo.Handlers

let api : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ route "/info" >=> handleInfo
             route "/panic" >=> handlePanic
             route "/echo" >=> handleEcho ]

let routes : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [ route "/healthz" >=> handleHealthz
             route "/version" >=> handleVersion
             subRoute "/api" api
             setStatusCode 404 >=> text "Not Found" ]
