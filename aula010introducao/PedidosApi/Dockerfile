FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["PedidosApi.csproj", "."]
RUN dotnet restore "./PedidosApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "PedidosApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PedidosApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PedidosApi.dll"]
