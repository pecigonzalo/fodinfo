module fodinfo.Config

open System
open System.Reflection

type Cofiguration =
    { hostname: string
      uiLogo: string
      uiColor: string
      uiMessage: string
      os: string
      arch: string
      runtime: string
      version: string
      num_cpu: string }

let getConfiguration =
    let version =
        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion
        |> string

    { hostname = Environment.MachineName
      uiLogo = "https://raw.githubusercontent.com/stefanprodan/podinfo/gh-pages/cuddle_clap.gif"
      uiColor = "#34577c"
      uiMessage = sprintf "greetings from fodinfo v%s" version
      os = string Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion
      arch = string Runtime.InteropServices.RuntimeInformation.ProcessArchitecture
      version = version
      runtime = Runtime.InteropServices.RuntimeInformation.FrameworkDescription
      num_cpu = string Environment.ProcessorCount }
