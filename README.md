# fodinfo

fodinfo is a tiny web application made with ~~Go~~ F# that showcases best practices of running microservices in Kubernetes. fodinfo is a F# port of the great work of [@stefanprodan](https://github.com/stefanprodan) in https://github.com/stefanprodan/podinfo to F#.

## Why?

Im using this as a projec to learn F# and potentially create a usefull template for F# project.

## Stack

- [Falco](https://github.com/pimbrouwers/Falco)

## Specifications:

- [x] Health checks (readiness and liveness)
- [ ] Instrumented with Prometheus **(In Progress)**
- [ ] 12-factor app **(In Progress)**
- [ ] File watcher for secrets and configmaps
- [ ] Tracing with Istio and Jaeger
- [ ] Linkerd service profile
- [ ] Structured logging
- [ ] Fault injection (random errors and latency)
- [ ] Swagger docs
- [ ] Helm and Kustomize installers
- [ ] End-to-End testing with Kubernetes Kind and Helm
- [ ] Kustomize testing with GitHub Actions and Open Policy Agent
- [ ] Multi-arch container image with Docker buildx and Github Actions
- [ ] CVE scanning with trivy
- [ ] Graceful shutdown on interrupt signals

## Web API:

- [ ] `GET /` prints runtime information
- [x] `GET /version` prints fodinfo version and git commit hash
- [x] `GET /metrics` return HTTP requests duration and Go runtime metrics
- [x] `GET /healthz` used by Kubernetes liveness probe
- [x] `GET /readyz` used by Kubernetes readiness probe
- [x] `POST /readyz/enable` signals the Kubernetes LB that this instance is ready to receive traffic
- [x] `POST /readyz/disable` signals the Kubernetes LB to stop sending requests to this instance
- [x] `GET /status/{code}` returns the status code
- [x] `GET /panic` crashes the process with exit code 255
- [ ] `POST /echo` forwards the call to the backend service and echos the posted content
- [x] `GET /env` returns the environment variables as a JSON array
- [x] `GET /headers` returns a JSON with the request HTTP headers
- [x] `GET /delay/{seconds}` waits for the specified period
- [ ] `POST /token` issues a JWT token valid for one minute `JWT=$(curl -sd 'anon' fodinfo:5000/token | jq -r .token)`
- [ ] `GET /token/validate` validates the JWT token `curl -H "Authorization: Bearer $JWT" fodinfo:5000/token/validate`
- [ ] `GET /configs` returns a JSON with configmaps and/or secrets mounted in the `config` volume
- [ ] `POST/PUT /cache/{key}` saves the posted content to Redis
- [ ] `GET /cache/{key}` returns the content from Redis if the key exists
- [ ] `DELETE /cache/{key}` deletes the key from Redis if exists
- [x] `POST /store` writes the posted content to disk at /data/hash and returns the SHA1 hash of the content
- [x] `GET /store/{hash}` returns the content of the file /data/hash if exists
- [ ] `GET /ws/echo` echos content via websockets `podcli ws ws://localhost:5000/ws/echo`
- [ ] `GET /chunked/{seconds}` uses `transfer-encoding` type `chunked` to give a partial response and then waits for the specified period
- [ ] `GET /swagger.json` returns the API Swagger docs, used for Linkerd service profiling and Gloo routes discovery
