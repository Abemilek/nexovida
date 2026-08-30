FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NexoVida.sln .
COPY WebApi.Models/WebApi.Models.csproj WebApi.Models/
COPY Services/WebApi.Interface/WebApi.Interface.csproj Services/WebApi.Interface/
COPY Services/WebApi.Implementation/WebApi.Implementation.csproj Services/WebApi.Implementation/
COPY WebApi/WebApi.csproj WebApi/
RUN dotnet restore NexoVida.sln

COPY . .
RUN dotnet publish WebApi/WebApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

USER app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "WebApi.dll"]
