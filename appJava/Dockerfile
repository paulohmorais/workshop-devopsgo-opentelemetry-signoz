# Use an official Maven image with Java 17 to build the application
FROM maven:3.8.4-openjdk-17 AS builder

# Set the working directory in the container
WORKDIR /app

# Copy the Maven project descriptor files
COPY pom.xml .

# Download dependencies and build the application JAR
RUN mvn dependency:go-offline
COPY src src
RUN mvn package -DskipTests

# Use an official OpenJDK 17 image to run the application
FROM openjdk:17-jdk-slim

# Set the working directory in the container
WORKDIR /app

# Copy the JAR file from the builder stage
COPY --from=builder /app/target/demo-0.0.1-SNAPSHOT.jar app.jar

# Expose the port on which the application will run
EXPOSE 8080

# Specify the command to run on container startup
CMD ["java", "-jar", "app.jar"]
