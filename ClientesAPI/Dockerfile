ARG PROJECT_NAME=ClientesAPI

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
ARG PROJECT_NAME

COPY Directory.Packages.props .
COPY ${PROJECT_NAME}/${PROJECT_NAME}.csproj ./${PROJECT_NAME}/
RUN dotnet restore ./${PROJECT_NAME}/${PROJECT_NAME}.csproj

COPY . .
RUN dotnet publish ./${PROJECT_NAME}/${PROJECT_NAME}.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ARG PROJECT_NAME

COPY --from=build /app/publish .

COPY ${PROJECT_NAME}/wait-for-it.sh .
RUN chmod +x wait-for-it.sh

EXPOSE 80

ENTRYPOINT ["./wait-for-it.sh", "sqlserverERP:1433", "-t", "60", "--", "dotnet", "ClientesAPI.dll"]