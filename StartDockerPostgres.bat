@echo off
docker run -d --name zidium-postgres -p 8080:80 -p 10000:10000 --mount type=bind,source=%CD%/Docker/zidium.postgres.appsettings.json,target=/zidium/zidium.appsettings.json local/zidium-simple:latest