module fodinfo.Config

open NetEscapades.Configuration.Validation
open System
open System.Reflection
open System.Runtime.InteropServices
open System.ComponentModel.DataAnnotations

let Runtime =
    let version =
        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion
        |> string

    {| hostname = Environment.MachineName
       os = string Environment.OSVersion.Platform
       arch = string RuntimeInformation.ProcessArchitecture
       version = version
       runtime = RuntimeInformation.FrameworkDescription
       num_cpu = string Environment.ProcessorCount |}

type Configuration() =
    let version = Runtime.version

    static member Name = "fodinfo"

    [<Url>]
    member val UILogo: string =
        "https://raw.githubusercontent.com/stefanprodan/podinfo/gh-pages/cuddle_clap.gif" with get, set

    [<RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")>]
    member val UIColor: string = "#34577c" with get, set

    member val UIMessage: string = sprintf "greetings from fodinfo v%s" version with get, set
    member val DataPath: string = "/data" with get, set
    // TODO: Implement as a list of backends to call
    member val BackendURL: string = "http://localhost:9898/store" with get, set

    interface IValidatable with
        member this.Validate() =
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties = true)
