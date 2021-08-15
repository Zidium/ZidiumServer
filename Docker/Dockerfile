FROM mcr.microsoft.com/dotnet/aspnet:5.0
RUN groupadd -r zidium && useradd --no-log-init -r -g zidium zidium
WORKDIR /zidium
RUN chown zidium $(pwd)
USER zidium
COPY --chown=zidium . .
EXPOSE 80
EXPOSE 10000
ENV ASPNETCORE_URLS=""
ENV ZIDIUM_CONFIG="/zidium/zidium.appsettings.json"
ENTRYPOINT ["/bin/bash", "./zidium.sh"]