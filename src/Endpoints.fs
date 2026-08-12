[<AutoOpen>]
module fodinfo.Endpoints

open fodinfo.Handlers
open Falco
open Falco.Routing

// podinfo serves the HTTP API at the root; only /api/info and /api/echo keep
// the /api prefix (used by the web UI and the Linkerd service profile)
let rootEndpoints : HttpEndpoint list =
    [ get "/healthz" handleHealthz
      get "/version" handleVersion
      post "/readyz/enable" handleReadyzEnable
      post "/readyz/disable" handleReadyzDisable
      get "/info" handleInfo
      get "/panic" handlePanic
      post "/echo" handleEcho
      get "/env" handleEnv
      get "/configs" handleConfig
      get "/status/{code}" handleStatus
      get "/headers" handleHeaders
      get "/delay/{seconds}" handleDelay
      post
          "/token"
          (Response.ofPlainText
              "issues a JWT token valid for one minute JWT=$(curl -sd 'anon' fodinfo:5000/token | jq -r .token)")
      get
          "/token/validate"
          (Response.ofPlainText
              """validates the JWT token curl -H "Authorization: Bearer $JWT" fodinfo:5000/token/validate""")
      post "/cache/{key}" (Response.ofPlainText "saves the posted content to Redis")
      get "/cache/{key}" (Response.ofPlainText "returns the content from Redis if the key exists")
      delete "/cache/{key}" (Response.ofPlainText "deletes the key from Redis if exists")
      post "/store" handleStorePost
      get "/store/{hash}" handleStoreGet
      get "/ws/echo" (Response.ofPlainText "echos content via websockets podcli ws ws://localhost:5000/ws/echo")
      get
          "/chunked/{seconds}"
          (Response.ofPlainText
              "uses transfer-encoding type chunked to give a partial response and then waits for the specified period")
      get
          "/swagger.json"
          (Response.ofPlainText
              "returns the API Swagger docs, used for Linkerd service profiling and Gloo routes discovery")
      get "/api/info" handleInfo
      post "/api/echo" handleEcho ]
