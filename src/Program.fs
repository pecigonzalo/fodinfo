module fodinfo.Program

open Falco
open Falco.HostBuilder
open Prometheus
open Serilog
open Serilog.Events
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Server.Kestrel.Core
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Options
open Microsoft.Extensions.DependencyInjection

let configureLogging (ctx: WebHostBuilderContext) (logger: LoggerConfiguration) =

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


let configureKestrel (ctx: WebHostBuilderContext) (kestrel: KestrelServerOptions) = kestrel.AddServerHeader <- false

let configureServices (ctx: WebHostBuilderContext) (services: IServiceCollection) =
    let configuration = ctx.Configuration

    // Enable NetEscapades.Configuration.Validation
    services.UseConfigurationValidation() |> ignore

    services.ConfigureValidatableSetting<Config.Configuration>(configuration.GetSection(Config.Configuration.Name))
    |> ignore

    // Add scoped service for easy access
    services.AddScoped<Config.Configuration>(
        (fun ctx ->
            ctx
                .GetRequiredService<IOptionsMonitor<Config.Configuration>>()
                .CurrentValue)
    )
    |> ignore

    services
        .AddHealthChecks()
        .AddCheck<HealthChecks.Toggle.HealthCheck>("Manual")
    |> ignore

    services.AddHttpClient() |> ignore
    services.AddRouting() |> ignore
    services.AddFalco() |> ignore

let configureApp (endpoints: HttpEndpoint list) (ctx: WebHostBuilderContext) (app: IApplicationBuilder) =
    match ctx.HostingEnvironment.IsDevelopment() with
    | true -> app.UseDeveloperExceptionPage()
    | false ->
        app.UseFalcoExceptionHandler(
            Response.withStatusCode 500
            >> Response.ofPlainText "Server error"
        )
    |> ignore

    [ app.UseSerilogRequestLogging()
      app.UseDefaultFiles()
      app.UseStaticFiles()
      app.UseRouting()
      app.UseMetricServer("/metrics")
      app.UseHealthChecks(PathString("/readyz"))
      app.UseHttpMetrics(
          configure =
              (fun options ->
                  options.AddLabel(labelName = "path", valueProvider = (fun ctx -> string ctx.Request.Path)))
      )
      app.UseFalco(endpoints) ]
    |> ignore

let configureWebHost (endpoints: HttpEndpoint list) (webHost: IWebHostBuilder) =
    webHost
        .UseSerilog(configureLogging)
        .ConfigureKestrel(configureKestrel)
        .ConfigureServices(configureServices)
        .Configure(configureApp endpoints)

[<EntryPoint>]
let main args =
    let exitCode = 0

    webHost args {
        configure configureWebHost
        endpoints rootEndpoints
    }

    Log.CloseAndFlush() // Wait for logs that may be in the buffer

    exitCode
