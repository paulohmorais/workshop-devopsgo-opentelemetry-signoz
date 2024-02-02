# Workshop Comunidade DevOpsGO

Repositório do workshop **Implementando observabilidade com Signoz e OpenTelemetry** realizado no HUB Goiás em Goiânia pela comunidade DevOpsGO. No dia 03/02/2024. 


## All apps need redis for cache content, to start redis with docker run

`docker run --name some-redis -p 6379:6379 -d redis`

Check redis is running with: `docker ps`

## Build and run C# APP 

Check file `appCSharp/Program.cs` and configure Redis Host

```
cd appCSharp
docker build -t appcsharp .
docker run -d --name appcsharp -p 3000:3000 appcsharp
```

## Build and run C# APP with OpenTelemetry Instrumentation

Check file `appCSharp/Program.cs` and add configure SIGNOZ-HOST

```
cd appCSharpOpenTelemetry
docker build -t appcsharp .
docker run -d --name appcsharp -p 3002:3000 appcsharp
```

## Build and run NodeJS APP 

Check file `appNodeJS/app.js` and add configure REDIS-HOST

```
cd appNodeJS
docker build -t appnodejs .
docker run -d --name appnodejs -p 3001:3001 appnodejs
```
## Build and run NodeJS APP with OpenTelemetry Instrumentation

Check file `appNodeJSOpenTelemetry/tracing.js` and add configure SIGNOZ-HOST

```
cd appNodeJS
docker build -t appnodejs .
docker run -d --name appnodejs -p 3001:3001 appnodejs
```

## Build and run Java APP 

Check file `appJava/src/main/resources/application.properties` and configure Redis Host

```
cd appJava
docker build -t appjava .
docker run -d --name appjava -p 3002:8080 appjava
```

## Build and run Java APP with OpenTelemetry Instrumentation

Check file `Dockerfile` and add configure the env OTEL_EXPORTER_OTLP_ENDPOINT

```
cd appJavaOpenTelemetry
docker build -t appjava .
docker run -d --name appjava -p 3002:8080 appjava
```