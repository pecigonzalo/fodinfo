FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src
COPY *.sln ./
COPY *.fsproj ./
COPY pkgs/prometheus-net/Prometheus.NetStandard/Prometheus.NetStandard.csproj /src/pkgs/prometheus-net/Prometheus.NetStandard/Prometheus.NetStandard.csproj
COPY pkgs/prometheus-net/Prometheus.AspNetCore/Prometheus.AspNetCore.csproj /src/pkgs/prometheus-net/Prometheus.AspNetCore/Prometheus.AspNetCore.csproj
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser  \
  && chown -R appuser /app
RUN mkdir /data && chown -R appuser /data
USER appuser

VOLUME [ "/data" ]

COPY --from=build /app .
ENTRYPOINT ["dotnet", "fodinfo.dll"]

HEALTHCHECK \
  --interval=5s --timeout=10s --start-period=5s --retries=3 \
  CMD [ "curl --fail http://localhost:8080/healthz" ]
