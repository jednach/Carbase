# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["Carbase.csproj", "./"]

RUN dotnet restore "Carbase.csproj"

COPY . .

RUN --mount=type=secret,id=sixlabors_license \
    dotnet publish "Carbase.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    -p:SixLaborsLicenseFile=/run/secrets/sixlabors_license


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Carbase.dll"]