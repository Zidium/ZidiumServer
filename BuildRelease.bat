@echo off
set msbuild.exe=c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe

rmdir /s /q Release

"%msbuild.exe%" Services\DispatcherHttpService\DispatcherHttpService.csproj /t:Build /p:Configuration=Release /v:minimal /p:DeployOnBuild=true /p:PublishProfile=CreateRelease.pubxml

"%msbuild.exe%" Api\1.0\ApiHttpService\ApiHttpService.1.0.csproj /t:Build /p:Configuration=Release /v:minimal /p:DeployOnBuild=true /p:PublishProfile=CreateRelease.pubxml

"%msbuild.exe%" UserAccount.AddIn\UserAccount.AddIn.csproj /t:Build /p:Configuration=Release /v:minimal /p:SolutionDir="%cd%"\

"%msbuild.exe%" UserAccount\UserAccount.csproj /t:Build /p:Configuration=Release /v:minimal /p:DeployOnBuild=true /p:PublishProfile=CreateRelease.pubxml

"%msbuild.exe%" Agent\Agent.csproj /t:Build /p:Configuration=Release /v:minimal
xcopy /E /Y /S Agent\bin\Release\*.pdb Release\Agent\
xcopy /E /Y /S Agent\bin\Release\NLog.config Release\Agent\
xcopy /E /Y /S Agent\bin\Release\Zidium.config Release\Agent\
xcopy /E /Y /S Agent\App.Release.config Release\Agent\Zidium.Agent.exe.config*
xcopy /E /Y /S Agent\bin\Release\*.exe Release\Agent\
xcopy /E /Y /S Agent\bin\Release\*.dll Release\Agent\

"%msbuild.exe%" Tools\DatabasesUpdate\DatabasesUpdate.csproj /t:Build /p:Configuration=Release /v:minimal
xcopy /E /Y /S Tools\DatabasesUpdate\bin\Release\*.pdb Release\DatabasesUpdate\
xcopy /E /Y /S Tools\DatabasesUpdate\bin\Release\NLog.config Release\DatabasesUpdate\
xcopy /E /Y /S Tools\DatabasesUpdate\bin\Release\DatabasesUpdate.exe.config Release\DatabasesUpdate\
xcopy /E /Y /S Tools\DatabasesUpdate\bin\Release\*.exe Release\DatabasesUpdate\
xcopy /E /Y /S Tools\DatabasesUpdate\bin\Release\*.dll Release\DatabasesUpdate\

pause