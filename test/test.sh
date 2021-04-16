#! /usr/bin/env sh

set -e

# wait for fodinfo
kubectl rollout status deployment/fodinfo --timeout=3m

# test fodinfo
helm test fodinfo
