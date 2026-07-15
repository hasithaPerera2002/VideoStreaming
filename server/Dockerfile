FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . . 
RUN dotnet publish -c Release -o app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 5144
ENV ASPNETCORE_URLS=http://0.0.0.0:5144
ENTRYPOINT ["dotnet", "VideoStreaming.dll"]