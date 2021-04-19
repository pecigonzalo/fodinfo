namespace fodinfo

module Config =
    open System
    open System.Reflection
    open System.Runtime.InteropServices

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

        let mutable uiLogo =
            "https://raw.githubusercontent.com/stefanprodan/podinfo/gh-pages/cuddle_clap.gif"

        let mutable uiColor = "#34577c"

        let mutable uiMessage =
            sprintf "greetings from fodinfo v%s" version

        let mutable dataPath = sprintf "/data"

        // TODO: Implement as a list of backends to call
        let mutable backendURL = sprintf "http://localhost:9898/store"

        member _.UILogo
            with get () = uiLogo
            and set (value) = uiLogo <- value

        member _.UIColor
            with get () = uiColor
            and set (value) = uiColor <- value

        member _.UIMessage
            with get () = uiMessage
            and set (value) = uiMessage <- value

        member _.DataPath
            with get () = dataPath
            and set (value) = dataPath <- value

        member _.BackendURL
            with get () = backendURL
            and set (value) = backendURL <- value
