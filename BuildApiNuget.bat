@echo off
dotnet pack Api\Api.csproj -o %~dp0Nuget\ -c release -v normal
pause