@echo off
rem Set enviroment variable ZIDIUM_MSBUILD to C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe

if not defined ZIDIUM_MSBUILD (
   echo Enviroment variable ZIDIUM_MSBUILD is not set
   pause
   exit /b 1
)

rmdir /s /q Release

"%ZIDIUM_MSBUILD%" /t:Services\DispatcherHttpService Zidium.sln /restore /p:Configuration="Publish" /v:minimal /p:DeployOnBuild=true /p:PublishProfile=CreateRelease.pubxml
del Release\DispatcherService\appsettings.json
xcopy Services\DispatcherHttpService\appsettings.release.json Release\DispatcherService\appsettings.json*

"%ZIDIUM_MSBUILD%" /t:Api\1_0\ApiHttpService_1_0 Zidium.sln /restore /p:Configuration="Publish" /v:minimal /p:DeployOnBuild=true /p:PublishProfile=CreateRelease.pubxml
del Release\ApiService\appsettings.json
xcopy Api\1.0\ApiHttpService\appsettings.release.json Release\ApiService\appsettings.json*

"%ZIDIUM_MSBUILD%" /t:UserAccount_AddIn Zidium.sln /restore /p:Configuration="Publish" /v:minimal /p:SolutionDir="%cd%"\

"%ZIDIUM_MSBUILD%" /t:UserAccount Zidium.sln /restore /p:Configuration="Publish" /v:minimal /p:DeployOnBuild=true /p:PublishProfile=CreateRelease.pubxml
xcopy /Y UserAccount.AddIn\bin\Zidium.UserAccount.AddIn.pdb Release\UserAccount\bin\
del Release\UserAccount\appsettings.json
xcopy UserAccount\appsettings.release.json Release\UserAccount\appsettings.json*

"%ZIDIUM_MSBUILD%" Agent\Agent.csproj /restore /t:Build /p:Configuration=Release /v:minimal
xcopy /Y Agent\bin\Release\*.pdb Release\Agent\
xcopy /Y Agent\bin\Release\NLog.config Release\Agent\
xcopy /Y Agent\bin\Release\Zidium.config Release\Agent\
xcopy /Y Agent\bin\Release\Zidium.Agent.exe.config Release\Agent\
xcopy /Y Agent\appsettings.release.json Release\Agent\appsettings.json*
xcopy /Y Agent\bin\Release\*.exe Release\Agent\
xcopy /Y Agent\bin\Release\*.dll Release\Agent\

"%ZIDIUM_MSBUILD%" Tools\DatabasesUpdate\DatabasesUpdate.csproj /restore /t:Build /p:Configuration=Release /v:minimal
xcopy /Y Tools\DatabasesUpdate\bin\Release\*.pdb Release\DatabasesUpdate\
xcopy /Y Tools\DatabasesUpdate\bin\Release\NLog.config Release\DatabasesUpdate\
xcopy /Y Tools\DatabasesUpdate\bin\Release\Zidium.DatabasesUpdate.exe.config Release\DatabasesUpdate\
xcopy /Y Tools\DatabasesUpdate\appsettings.release.json Release\DatabasesUpdate\appsettings.json*
xcopy /Y Tools\DatabasesUpdate\bin\Release\*.exe Release\DatabasesUpdate\
xcopy /Y Tools\DatabasesUpdate\bin\Release\*.dll Release\DatabasesUpdate\

pause