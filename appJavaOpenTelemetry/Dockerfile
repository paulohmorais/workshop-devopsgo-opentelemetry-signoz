# Use an official Maven image with Java 17 to build the application
FROM maven:3.8.4-openjdk-17 AS builder
WORKDIR /app
COPY pom.xml .
RUN mvn dependency:go-offline
COPY src src
RUN mvn package -DskipTests

FROM openjdk:17-jdk-slim
WORKDIR /app
COPY --from=builder /app/target/demo-0.0.1-SNAPSHOT.jar app.jar
RUN apt update && apt install wget -y && wget https://github.com/open-telemetry/opentelemetry-java-instrumentation/releases/latest/download/opentelemetry-javaagent.jar
EXPOSE 8080
ENTRYPOINT OTEL_EXPORTER_OTLP_ENDPOINT="http://SIGNOZ-HOST:4318" OTEL_RESOURCE_ATTRIBUTES=service.name=javaApp java -javaagent:$PWD/opentelemetry-javaagent.jar -jar app.jar