#! /usr/bin/env sh

# add jetstack repository
helm repo add jetstack https://charts.jetstack.io || true

# install cert-manager
helm upgrade --install cert-manager jetstack/cert-manager \
    --set installCRDs=true \
    --namespace default

# wait for cert manager
kubectl rollout status deployment/cert-manager --timeout=2m
kubectl rollout status deployment/cert-manager-webhook --timeout=2m
kubectl rollout status deployment/cert-manager-cainjector --timeout=2m

# install self-signed certificate
cat << 'EOF' | kubectl apply -f -
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: self-signed
spec:
  selfSigned: {}
EOF

# install fodinfo with tls enabled
helm upgrade --install fodinfo ./charts/fodinfo \
    --set image.repository=test/fodinfo \
    --set image.tag=latest \
    --set tls.enabled=false \
    --set certificate.create=true \
    --namespace=default
