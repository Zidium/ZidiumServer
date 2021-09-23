@echo off
docker buildx create --name ZidiumBuilder --use
docker buildx build --platform linux/amd64,linux/arm64 --push -t zidiumserver/zidium-simple:latest .