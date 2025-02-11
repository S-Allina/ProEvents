FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore WebApp/WebApp.csproj
RUN dotnet build WebApp/WebApp.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS publish
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "WebApp.dll"]