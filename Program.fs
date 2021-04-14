module fodinfo.App

open System
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Prometheus
open Giraffe
open Giraffe.EndpointRouting

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
    app.UseGiraffeErrorHandler(errorHandler) |> ignore
    app.UseRouting() |> ignore
    app.UseMetricServer("/metrics") |> ignore

    app.UseHealthChecks(PathString("/readyz"))
    |> ignore

    app.UseDefaultFiles() |> ignore
    app.UseStaticFiles() |> ignore

    app.UseHttpMetrics(
        configure =
            (fun options -> options.AddLabel(labelName = "path", valueProvider = (fun ctx -> string ctx.Request.Path)))
    )
    |> ignore

    app.UseGiraffe(Endpoints.endpoints) |> ignore

let configureServices (services: IServiceCollection) =
    services.AddHealthChecks() |> ignore
    services.AddRouting() |> ignore
    services.AddGiraffe() |> ignore


let configureLogging (logging: ILoggingBuilder) =
    logging.AddConsole().AddDebug() |> ignore

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
