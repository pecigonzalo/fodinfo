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
open Serilog


// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex: Exception) (logger: Microsoft.Extensions.Logging.ILogger) =
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

    match env.IsDevelopment() with
    | true -> app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler(errorHandler)
    |> ignore

    app.UseSerilogRequestLogging() |> ignore
    app.UseDefaultFiles() |> ignore
    app.UseStaticFiles() |> ignore
    app.UseRouting() |> ignore
    app.UseMetricServer("/metrics") |> ignore

    app.UseHealthChecks(PathString("/readyz"))
    |> ignore

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


let configureLogging (ctx: HostBuilderContext) (logger: LoggerConfiguration) =
    logger.Enrich.FromLogContext() |> ignore

    // logger.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    // |> ignore

    match ctx.HostingEnvironment.IsDevelopment() with
    | true -> logger.WriteTo.Console(theme = Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
    | false -> logger.WriteTo.Console(Formatting.Compact.RenderedCompactJsonFormatter())
    |> ignore

[<EntryPoint>]
let main args =
    let exitCode = 0

    Host
        .CreateDefaultBuilder(args)
        .UseSerilog(configureLogging)
        .ConfigureWebHostDefaults(fun webHostBuilder ->
            webHostBuilder
                .Configure(configureApp)
                .ConfigureServices(configureServices)
            |> ignore)
        .Build()
        .Run()

    exitCode
