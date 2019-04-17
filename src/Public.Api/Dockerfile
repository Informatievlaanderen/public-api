FROM microsoft/dotnet:2.2.0-runtime-deps

LABEL maintainer "agentschap Informatie Vlaanderen <informatie.vlaanderen@vlaanderen.be>"

COPY / /app
WORKDIR /app

ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_INTEGRATIONS=/opt/datadog/integrations.json

RUN curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v1.0.0/datadog-dotnet-apm_1.0.0_amd64.deb && \
    sudo dpkg -i ./datadog-dotnet-apm_1.0.0_amd64.deb

EXPOSE 2080/tcp
ENV ASPNETCORE_URLS http://*:2080

ENV DD_SERVICE_NAME=public-api
ENTRYPOINT ["./Public.Api"]
