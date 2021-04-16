#! /usr/bin/env sh

set -e

# build the docker file
DOCKER_BUILDKIT=1 docker build --tag test/fodinfo .
