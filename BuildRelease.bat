@echo off

echo Clearing release folder
rmdir /s /q Release

echo.
echo Building Dispatcher
echo.
dotnet publish "Dispatcher\Dispatcher.csproj" -v:minimal -c:Release -o:.\Release\Dispatcher

echo.
echo Building UserAccount
echo.
dotnet publish "UserAccount\UserAccount.csproj" -v:minimal -c:Release -o:.\Release\UserAccount

echo.
echo Building Agent.ConsoleApplication
echo.
dotnet publish "Agent\Agent.ConsoleApplication\Agent.ConsoleApplication.csproj" -v:minimal -c:Release -o:.\Release\Agent\Console

echo.
echo Building Agent.WindowsService
echo.
dotnet publish "Agent\Agent.WindowsService\Agent.WindowsService.csproj" -f:net6.0 -r:win-x64 --no-self-contained -v:minimal -c:Release -o:.\Release\Agent\WindowsService

echo.
echo Building DatabaseUpdater
echo.
dotnet publish "DatabaseUpdater\DatabaseUpdater.csproj" -v:minimal -c:Release -o:.\Release\DatabaseUpdater

echo.
echo Done!