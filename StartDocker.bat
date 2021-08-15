@echo off
docker run -d -p 10001:80 -p 10000:10000 --mount type=bind,source=%CD%/Docker/zidium.appsettings.json,target=/zidium/zidium.appsettings.json zidium/simple:latest