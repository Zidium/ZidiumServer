@echo off
docker login
docker build -t zidiumteam/zidium-simple:2.0.0 -t zidiumteam/zidium-simple:latest .
docker push zidiumteam/zidium-simple:2.0.0
docker push zidiumteam/zidium-simple:latest