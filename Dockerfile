FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /build
COPY fodinfo.sln ./
COPY src/fodinfo.fsproj ./src/
RUN dotnet restore

COPY . .
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

# curl is needed by the HEALTHCHECK below
RUN apt-get update \
  && apt-get install -y --no-install-recommends curl \
  && rm -rf /var/lib/apt/lists/*

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
  CMD ["curl", "--fail", "http://localhost:5000/healthz"]
