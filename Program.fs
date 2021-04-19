module fodinfo.Program

open Falco
open Falco.Routing
open Falco.HostBuilder
open Prometheus
open Serilog
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Server.Kestrel.Core

let configureLogging (ctx: WebHostBuilderContext) (logger: LoggerConfiguration) =
    logger.Enrich.FromLogContext() |> ignore
    logger.MinimumLevel.Debug() |> ignore

    logger.ReadFrom.Configuration(ctx.Configuration)
    |> ignore

    match ctx.HostingEnvironment.IsDevelopment() with
    | true -> logger.WriteTo.Console(theme = Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
    | false -> logger.WriteTo.Console(Formatting.Compact.RenderedCompactJsonFormatter())
    |> ignore

let configureKestrel (ctx: WebHostBuilderContext) (kestrel: KestrelServerOptions) = kestrel.AddServerHeader <- false

let configureServices (ctx: WebHostBuilderContext) (services: IServiceCollection) =
    services.Configure<Config.Configuration>(ctx.Configuration.GetSection("fodinfo"))
    |> ignore

    services
        .AddHealthChecks()
        .AddCheck<HealthChecks.Toggle.HealthCheck>("Manual")
    |> ignore

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

        endpoints [ get "/healthz" Handlers.Healthz.handleHealthz
                    get "/version" Handlers.Version.handleVersion
                    get "/api/info" Handlers.Info.handleInfo
                    get "/api/panic" Handlers.Panic.handlePanic
                    get "/api/echo" Handlers.Echo.handleEcho
                    get "/api/env" Handlers.Env.handleEnv
                    get "/api/config" Handlers.Config.handleConfig
                    post "/api/readyz/enable" Handlers.Readyz.handleReadyzEnable
                    post "/api/readyz/disable" Handlers.Readyz.handleReadyzDisable
                    get "/api/status/{code}" Handlers.Status.handleStatus

                    get "/api/headers" Handlers.Headers.handleHeaders
                    get "/api/delay/{seconds}" Handlers.Delay.handleDelay
                    post
                        "/api/token"
                        (Response.ofPlainText
                            "issues a JWT token valid for one minute JWT=$(curl -sd 'anon' podinfo:9898/token | jq -r .token)")
                    get
                        "/api/token/validate"
                        (Response.ofPlainText
                            """validates the JWT token curl -H "Authorization: Bearer $JWT" podinfo:9898/token/validate""")
                    post "/api/cache/{key}" (Response.ofPlainText "saves the posted content to Redis")
                    get "/api/cache/{key}" (Response.ofPlainText "returns the content from Redis if the key exists")
                    delete "/api/cache/{key}" (Response.ofPlainText "deletes the key from Redis if exists")
                    post "/api/store" Handlers.Store.handleStorePost
                    get "/api/store/{hash}" Handlers.Store.handleStoreGet
                    get
                        "/api/ws/echo"
                        (Response.ofPlainText "echos content via websockets podcli ws ws://localhost:9898/ws/echo")
                    get
                        "/api/chunked/{seconds}"
                        (Response.ofPlainText
                            "uses transfer-encoding type chunked to give a partial response and then waits for the specified period")
                    get
                        "/api/swagger.json"
                        (Response.ofPlainText
                            "returns the API Swagger docs, used for Linkerd service profiling and Gloo routes discovery") ]
    }

    exitCode
