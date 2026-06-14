FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["Src/Services/WebApi/WebApi.Presentation/WebApi.Presentation.csproj", "Src/Services/WebApi/WebApi.Presentation/"]
COPY ["Src/Services/WebApi/WebApi.infrastructure/WebApi.infrastructure.csproj", "Src/Services/WebApi/WebApi.infrastructure/"]
COPY ["Src/Services/WebApi/WebApi.Application/WebApi.Application.csproj", "Src/Services/WebApi/WebApi.Application/"]
COPY ["Src/Services/WebApi/WebApi.Domain/WebApi.Domain.csproj", "Src/Services/WebApi/WebApi.Domain/"]
COPY ["Src/Utilities/Domain/Domain.csproj", "Src/Utilities/Domain/"]
RUN dotnet restore "Src/Services/WebApi/WebApi.Presentation/WebApi.Presentation.csproj"
COPY . .
WORKDIR "/src/Src/Services/WebApi/WebApi.Presentation"
RUN dotnet build "./WebApi.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.Presentation.dll"]
