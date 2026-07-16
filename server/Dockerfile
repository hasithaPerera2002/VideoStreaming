FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . . 
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY --from=base /app .
EXPOSE 5144
ENV ASPNETCORE_URLS=http://0.0.0.0:5144
ENTRYPOINT ["dotnet", "server.dll"]