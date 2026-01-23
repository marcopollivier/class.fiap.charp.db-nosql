# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY RedisExamplesApi.csproj .
RUN dotnet restore "RedisExamplesApi.csproj"
COPY . .
RUN dotnet build "RedisExamplesApi.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "RedisExamplesApi.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
ENTRYPOINT ["dotnet", "RedisExamplesApi.dll"]
