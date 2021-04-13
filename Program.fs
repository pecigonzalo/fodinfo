module fodinfo.App

open System
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Prometheus

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")

    clearResponse
    >=> setStatusCode 500
    >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureApp (app: IApplicationBuilder) =
    let env =
        app.ApplicationServices.GetService<IWebHostEnvironment>()

    let defaultOptions =
        match env.IsDevelopment() with
        | true -> app.UseDeveloperExceptionPage()
        | false ->
            app
                .UseGiraffeErrorHandler(errorHandler)
                .UseHttpsRedirection()

    defaultOptions
        .UseHttpMetrics()
        .UseMetricServer()
        .UseHealthChecks(PathString("/readyz"))
        .UseDefaultFiles()
        .UseStaticFiles()
        .UseResponseCaching()
        .UseGiraffe(fodinfo.Routing.routes)

let configureServices (services: IServiceCollection) =
    services.AddHealthChecks().ForwardToPrometheus()
    |> ignore

    services.AddCors() |> ignore
    services.AddGiraffe() |> ignore


let configureLogging (builder: ILoggingBuilder) =
    builder.AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main args =
    Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .Configure(Action<IApplicationBuilder> configureApp)
                .ConfigureServices(configureServices)
                .ConfigureLogging(configureLogging)
            |> ignore)
        .Build()
        .Run()

    0
