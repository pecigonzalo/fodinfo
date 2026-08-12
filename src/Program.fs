module fodinfo.Program

open Falco
open Falco.Routing
open Prometheus
open Serilog
open Serilog.Events
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Server.Kestrel.Core
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

let configureLogging (ctx: HostBuilderContext) (logger: LoggerConfiguration) =

    logger.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    |> ignore

    // TODO: Review using Async writters. This is currently used as the performance is otherwise impacted
    // by printting logs to console.
    match ctx.HostingEnvironment.IsDevelopment() with
    | true ->
        logger.WriteTo.Async(
            (fun a ->
                a.Console(theme = Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                |> ignore)
        )
        |> ignore
    | false ->
        logger.WriteTo.Async(
            (fun a ->
                a.Console(Formatting.Compact.RenderedCompactJsonFormatter())
                |> ignore)
        )
        |> ignore

    logger.Enrich.FromLogContext() |> ignore

    logger.ReadFrom.Configuration(ctx.Configuration)
    |> ignore


let configureKestrel (kestrel: KestrelServerOptions) = kestrel.AddServerHeader <- false

let configureServices (ctx: HostBuilderContext) (services: IServiceCollection) =
    services.Configure<Config.Configuration>(ctx.Configuration.GetSection("fodinfo"))
    |> ignore

    services
        .AddHealthChecks()
        .AddCheck<HealthChecks.Toggle.HealthCheck>("Manual")
    |> ignore

    services.AddHttpClient() |> ignore
    services.AddRouting() |> ignore

let configureApp (endpoints: HttpEndpoint list) (app: WebApplication) =
    if app.Environment.IsDevelopment() then
        app.UseDeveloperExceptionPage() |> ignore
    else
        app.UseFalcoExceptionHandler(
            Response.withStatusCode 500
            >> Response.ofPlainText "Server error"
        )
        |> ignore

    app.UseSerilogRequestLogging() |> ignore
    app.UseDefaultFiles() |> ignore
    app.UseStaticFiles() |> ignore
    app.UseRouting() |> ignore
    app.UseMetricServer("/metrics") |> ignore
    app.UseHealthChecks(PathString("/readyz")) |> ignore
    app.UseHttpMetrics(
        configure =
            (fun options ->
                options.AddCustomLabel("path", fun ctx -> string ctx.Request.Path))
    )
    |> ignore
    app.UseFalco(endpoints) |> ignore

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Host.UseSerilog(configureLogging) |> ignore
    builder.Services.Configure<KestrelServerOptions>(configureKestrel) |> ignore
    builder.Host.ConfigureServices(configureServices) |> ignore

    let app = builder.Build()
    configureApp rootEndpoints app
    app.Run()

    Log.CloseAndFlush() // Wait for logs that may be in the buffer

    0
