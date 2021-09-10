FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "Dispatcher/Dispatcher.csproj" -v:minimal -c:Release -o:/Release/Dispatcher
RUN dotnet publish "UserAccount/UserAccount.csproj" -v:minimal -c:Release -o:/Release/UserAccount
RUN dotnet publish "Agent/Agent.ConsoleApplication/Agent.ConsoleApplication.csproj" -v:minimal -c:Release -o:/Release/Agent
COPY ["Docker/dispatcher.appsettings.prod.json", "/Release/Dispatcher/appsettings.prod.json"]
COPY ["Docker/user-account.appsettings.prod.json", "/Release/UserAccount/appsettings.prod.json"]
COPY ["Docker/agent.appsettings.prod.json", "/Release/Agent/appsettings.prod.json"]
COPY ["Docker/zidium.sh", "/Release/"]

FROM mcr.microsoft.com/dotnet/aspnet:5.0
RUN groupadd -r zidium && useradd --no-log-init -r -g zidium zidium
WORKDIR /zidium
RUN chown zidium:zidium $(pwd)
RUN mkdir /zidium/sqlite && chown zidium:zidium /zidium/sqlite
RUN mkdir /zidium/log && chown zidium:zidium /zidium/log
VOLUME /zidium/sqlite
VOLUME /zidium/log
# USER zidium
COPY --chown=zidium --from=build /Release/ .
EXPOSE 80
EXPOSE 10000
ENV ASPNETCORE_URLS=""
ENV ZIDIUM_CONFIG="/zidium/zidium.appsettings.json"
ENTRYPOINT ["/bin/bash", "./zidium.sh"]