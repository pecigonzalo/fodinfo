#! /usr/bin/env sh

set -e

SCRIPT_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd -P)

# run the build
$SCRIPT_DIR/build.sh

# create the k3d cluster
k3d cluster create || true

# load the docker image
k3d image import test/fodinfo:latest

# run the deploy
$SCRIPT_DIR/deploy.sh

# run the tests
$SCRIPT_DIR/test.sh
