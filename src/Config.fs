module fodinfo.Config

open System
open System.Reflection
open System.Runtime.InteropServices

let private informationalVersion =
    Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        .InformationalVersion
    |> string

/// Semantic version: informational version without the git revision suffix.
let Version =
    match informationalVersion.Split('+', 2) with
    | [| version; _ |] -> version
    | _ -> informationalVersion

/// Git revision embedded by the build via `-p:SourceRevisionId` (see Makefile).
/// The SDK appends it to the informational version as a `+<revision>` suffix.
let Revision =
    match informationalVersion.Split('+', 2) with
    | [| _; revision |] when not (String.IsNullOrWhiteSpace revision) -> revision
    | _ -> "unknown"

let Runtime =
    {| hostname = Environment.MachineName
       os = string Environment.OSVersion.Platform
       arch = string RuntimeInformation.ProcessArchitecture
       version = Version
       runtime = RuntimeInformation.FrameworkDescription
       num_cpu = string Environment.ProcessorCount |}

type Configuration() =
    member _.Name = "fodinfo"

    member val UILogo: string =
        "https://raw.githubusercontent.com/stefanprodan/podinfo/gh-pages/cuddle_clap.gif" with get, set

    member val UIColor: string = "#34577c" with get, set
    member val UIMessage: string = sprintf "greetings from fodinfo v%s" Version with get, set
    member val DataPath: string = "/data" with get, set
    // TODO: Implement as a list of backends to call
    member val BackendURL: string = "http://localhost:9898/store" with get, set
