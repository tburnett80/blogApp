
FROM microsoft/dotnet:2.1-sdk-nanoserver-sac2016 AS build
#FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet publish BlogApp.sln -c Release -o /app

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-sac2016
#FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
WORKDIR /app
EXPOSE 80
COPY --from=build /app .
ENTRYPOINT ["dotnet", "BlogApp.Server.dll"]
