@echo off
rem Set enviroment variable ZIDIUM_MSBUILD to C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe

if not defined ZIDIUM_MSBUILD (
   echo Enviroment variable ZIDIUM_MSBUILD is not set
   pause
   exit /b 1
)

rmdir /s /q Release

"%ZIDIUM_MSBUILD%" Services\DispatcherHttpService\DispatcherHttpService.csproj /t:Build /p:Configuration=Release /v:minimal /p:DeployOnBuild=true /p:PublishProfile=CreateRelease.pubxml

"%ZIDIUM_MSBUILD%" Api\1.0\ApiHttpService\ApiHttpService.1.0.csproj /t:Build /p:Configuration=Release /v:minimal /p:DeployOnBuild=true /p:PublishProfile=CreateRelease.pubxml

"%ZIDIUM_MSBUILD%" UserAccount.AddIn\UserAccount.AddIn.csproj /t:Build /p:Configuration=Release /v:minimal /p:SolutionDir="%cd%"\

"%ZIDIUM_MSBUILD%" UserAccount\UserAccount.csproj /t:Build /p:Configuration=Release /v:minimal /p:DeployOnBuild=true /p:PublishProfile=CreateRelease.pubxml
xcopy /Y UserAccount.AddIn\bin\Zidium.UserAccount.AddIn.pdb Release\UserAccount\bin\

"%ZIDIUM_MSBUILD%" Agent\Agent.csproj /t:Build /p:Configuration=Release /v:minimal
xcopy /Y Agent\bin\Release\*.pdb Release\Agent\
xcopy /Y Agent\bin\Release\NLog.config Release\Agent\
xcopy /Y Agent\bin\Release\Zidium.config Release\Agent\
xcopy /Y Agent\App.Release.config Release\Agent\Zidium.Agent.exe.config*
xcopy /Y Agent\bin\Release\*.exe Release\Agent\
xcopy /Y Agent\bin\Release\*.dll Release\Agent\

"%ZIDIUM_MSBUILD%" Tools\DatabasesUpdate\DatabasesUpdate.csproj /t:Build /p:Configuration=Release /v:minimal
xcopy /Y Tools\DatabasesUpdate\bin\Release\*.pdb Release\DatabasesUpdate\
xcopy /Y Tools\DatabasesUpdate\bin\Release\NLog.config Release\DatabasesUpdate\
xcopy /Y Tools\DatabasesUpdate\bin\Release\DatabasesUpdate.exe.config Release\DatabasesUpdate\
xcopy /Y Tools\DatabasesUpdate\bin\Release\*.exe Release\DatabasesUpdate\
xcopy /Y Tools\DatabasesUpdate\bin\Release\*.dll Release\DatabasesUpdate\

pause