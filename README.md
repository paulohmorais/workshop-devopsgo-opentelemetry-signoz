# Workshop Comunidade DevOpsGO

Repositório do workshop **Implementando observabilidade com Signoz e OpenTelemetry** realizado no HUB Goiás em Goiânia pela comunidade DevOpsGO. No dia 03/02/2024. 

## To running SigNoz self-hosted with docker

```
git clone -b main https://github.com/SigNoz/signoz.git && cd signoz/deploy/
./install.sh
```

On your browser go to http://localhost:3301/.

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
docker run -d --name appcsharp -p 3000:3000 appcsharp
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

## Monitoring k8s with opentelemetry

Install helm chart

```
helm repo add signoz https://charts.signoz.io
helm repo update
helm install my-release signoz/k8s-infra  \
--set otelCollectorEndpoint=<IP-or-Endpoint-of-SigNoz-OtelCollector>:4317
```

## Collector linux metrics and logs

Install otel-collector-contrib on linux host

```
sudo apt-get update
sudo apt-get -y install wget
wget https://github.com/open-telemetry/opentelemetry-collector-releases/releases/download/v0.93.0/otelcol-contrib_0.93.0_linux_amd64.deb
sudo dpkg -i otelcol-contrib_0.93.0_linux_amd64.deb
```
To collect host logs at `/var/log/` we need root permission. So edit the file `/lib/systemd/system/otelcol-contrib.service` for set User and Group as root:

```
User=root
Group=root
```

After restart otel-contrib with new config:

```
sudo systemctl daemon-reload
sudo systemctl restart otelcol-contrib
```

Now edit the config file `/etc/otelcol-contrib/config.yaml` add the below config, with otel collector endpoint:


```
extensions:
  health_check:
  pprof:
    endpoint: 0.0.0.0:1777
  zpages:
    endpoint: 0.0.0.0:55679

receivers:
  hostmetrics:
    collection_interval: 10s
    scrapers:
      cpu:
      disk:
      filesystem:
      load:
      memory:
      network:
      paging:
  filelog:
    include: [ /var/log/*log ]
    start_at: end

processors:
  batch:
  resourcedetection:
    detectors: [env, system]
  cumulativetodelta:

exporters:
  otlp:
    endpoint: <IP-or-Endpoint-of-SigNoz-OtelCollector>:4317
    tls:
      insecure: false
    headers:
      "signoz-access-token": "<SIGNOZ_INGESTION_KEY>"

service:
  pipelines:
    metrics:
      receivers: [hostmetrics]
      processors: [cumulativetodelta, batch, resourcedetection]
      exporters: [otlp]
    logs:
      receivers: [filelog]
      processors: [batch, resourcedetection]
      exporters: [otlp]

  extensions: [health_check, pprof, zpages]

```

And restart OpenTelemetry Contrib Collector:

```
sudo systemctl restart otelcol-contrib
```

## Export dashboard from dashboard folder