@echo off
docker run -d -p 10001:80 -p 10000:10000 -e "ZIDIUM_webSite=http://localhost:10001" -e "ZIDIUM_secretKey=XXX" zidium/simple:latest