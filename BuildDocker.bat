@echo off

call BuildRelease.bat

echo Clearing docker release folder
rmdir /s /q DockerRelease

echo Populating docker release folder
mkdir DockerRelease

xcopy /E /I Release\Dispatcher DockerRelease\Dispatcher
xcopy /E /I Release\Agent\Console DockerRelease\Agent
xcopy /E /I Release\UserAccount DockerRelease\UserAccount

copy /B Docker\dispatcher.appsettings.prod.json DockerRelease\Dispatcher\appsettings.prod.json
copy /B Docker\user-account.appsettings.prod.json DockerRelease\UserAccount\appsettings.prod.json
copy /B Docker\agent.appsettings.prod.json DockerRelease\Agent\appsettings.prod.json
copy /B Docker\zidium.sh DockerRelease\

echo Building docker image
docker build -t zidium/simple:latest -f Docker/Dockerfile DockerRelease