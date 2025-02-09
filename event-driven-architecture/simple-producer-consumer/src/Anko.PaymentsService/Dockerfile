# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore Anko.PaymentsService/Anko.PaymentsService.csproj
# Build and publish a release
RUN dotnet publish Anko.PaymentsService/Anko.PaymentsService.csproj -c Release -o out

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "Anko.PaymentsService.dll"]
