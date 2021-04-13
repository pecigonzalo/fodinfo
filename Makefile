sha := $(shell git rev-parse --short HEAD)

build:
	dotnet build \
		-p:VersionSuffix=rc \
		-p:SourceRevisionId=$(sha) \
		-p:RepositoryUrl=https://github.com/pecigonzalo/fodinfo \
		-p:RepositoryType=git
