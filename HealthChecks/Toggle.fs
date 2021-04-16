namespace fodinfo.HealthChecks

module Toggle =
    open Microsoft.Extensions.Diagnostics.HealthChecks

    type ToggleState =
        | Enabled
        | Disabled

    type ToggleMessage =
        | GetState of replyChannel: AsyncReplyChannel<ToggleState>
        | Enable
        | Disable

    type ToggleAPI =
        { GetState: unit -> ToggleState
          Enable: unit -> unit
          Disable: unit -> unit }

    let Agent =
        let mailbox =
            MailboxProcessor.Start
                (fun inbox ->
                    let rec messageLoop state =
                        async {
                            let! msg = inbox.Receive()

                            match msg with
                            | GetState replyChannel ->
                                replyChannel.Reply state
                                return! messageLoop state
                            | Enable -> return! messageLoop Enabled
                            | Disable -> return! messageLoop Disabled

                            return! messageLoop state
                        }

                    messageLoop Enabled)

        { GetState = fun () -> mailbox.PostAndReply GetState
          Enable = fun () -> mailbox.Post Enable
          Disable = fun () -> mailbox.Post Disable }

    type HealthCheck() =
        interface IHealthCheck with
            member _.CheckHealthAsync(ctx, cancellationToken) =
                async {
                    return
                        match Agent.GetState() with
                        | Enabled -> HealthCheckResult.Healthy("Manual switch is Healthy")
                        | Disabled -> HealthCheckResult.Unhealthy("Manual switch is Unhealthy")
                }
                |> Async.StartAsTask
