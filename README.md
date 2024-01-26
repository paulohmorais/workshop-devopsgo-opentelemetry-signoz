# Workshop Comunidade DevOpsGO

Repositório do workshop **Implementando observabilidade com Signoz e OpenTelemetry** realizado no HUB Goiás em Goiânia pela comunidade DevOpsGO. No dia 03/02/2024. 


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