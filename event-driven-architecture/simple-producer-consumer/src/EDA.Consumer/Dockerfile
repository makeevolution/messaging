# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore EDA.Consumer/EDA.Consumer.csproj
# Build and publish a release
RUN dotnet publish EDA.Consumer/EDA.Consumer.csproj -c Release -o out

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "EDA.Consumer.dll"]
