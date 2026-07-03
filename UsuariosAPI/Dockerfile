FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Directory.Packages.props .
COPY UsuariosAPI/UsuariosAPI.csproj ./UsuariosAPI/
RUN dotnet restore ./UsuariosAPI/UsuariosAPI.csproj

COPY . .
RUN dotnet publish ./UsuariosAPI/UsuariosAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

COPY UsuariosAPI/wait-for-it.sh .
RUN chmod +x wait-for-it.sh

EXPOSE 80

ENTRYPOINT ["./wait-for-it.sh", "sqlserverERP:1433", "-t", "60", "--", "dotnet", "UsuariosAPI.dll"]