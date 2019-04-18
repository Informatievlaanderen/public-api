FROM microsoft/dotnet:2.2.0-runtime-deps

LABEL maintainer "agentschap Informatie Vlaanderen <informatie.vlaanderen@vlaanderen.be>"

COPY / /app
WORKDIR /app

RUN apt-get update && \
    apt-get install curl jq -y && \
    chmod +x ./init.sh

EXPOSE 2080/tcp
ENV ASPNETCORE_URLS http://*:2080

ENTRYPOINT ["./init.sh"]
