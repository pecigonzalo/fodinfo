TAG?=latest
NAME:=fodinfo
DOCKER_REPOSITORY:=ghcr.io/pecigonzalo
DOCKER_IMAGE_NAME:=$(DOCKER_REPOSITORY)/$(NAME)
GIT_COMMIT:=$(shell git describe --dirty --always)
VERSION:=$(shell grep -oP '(?<=<VersionPrefix>).*?(?=</VersionPrefix>)' src/fodinfo.fsproj)

default: build-container

.PHONY: test
test:
	test/e2e.sh

build:
	dotnet build \
		-p:VersionSuffix=rc \
		-p:SourceRevisionId=$(GIT_COMMIT) \
		-p:RepositoryUrl=https://github.com/pecigonzalo/fodinfo \
		-p:RepositoryType=git

build-charts:
	helm lint charts/*
	helm package charts/*

build-container:
	docker build -t $(DOCKER_IMAGE_NAME):$(VERSION) .

push-container:
	docker tag $(DOCKER_IMAGE_NAME):$(VERSION) $(DOCKER_IMAGE_NAME):latest
	docker push $(DOCKER_IMAGE_NAME):$(VERSION)
	docker push $(DOCKER_IMAGE_NAME):latest

release:
	git tag $(VERSION)
	git push origin $(VERSION)
