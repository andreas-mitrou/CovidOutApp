FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CovidOutApp.Web.csproj", ""]
RUN dotnet restore "./CovidOutApp.Web.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CovidOutApp.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CovidOutApp.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CovidOutApp.Web.dll"]