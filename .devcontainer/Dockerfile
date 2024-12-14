# Use an official .NET SDK image as the base
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base

# Install Vim
RUN apt-get update && apt-get install -y vim

# Set the working directory in the container
WORKDIR /workspace
