@echo off
docker run -d --name zidium-sqlite -p 8080:80 -p 10000:10000 -e "ZIDIUM_webSite=http://localhost:8080" -e "ZIDIUM_secretKey=XXX" -v /zidium/sqlite zidium/simple:latest