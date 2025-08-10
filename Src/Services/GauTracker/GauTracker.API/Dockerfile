# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["Src/Services/GauTracker/GauTracker.API/GauTracker.API.csproj", "Src/Services/GauTracker/GauTracker.API/"]
COPY ["Src/Common/Identification.Core/Identification.Core.csproj", "Src/Common/Identification.Core/"]
COPY ["Src/Common/Identification/Identification.Base/Identification.Base.csproj", "Src/Common/Identification/Identification.Base/"]
COPY ["Src/Common/Observability/Observability.csproj", "Src/Common/Observability/"]
COPY ["Src/Services/GauTracker/GauTracker.Infrastructure/GauTracker.Infrastructure.csproj", "Src/Services/GauTracker/GauTracker.Infrastructure/"]
COPY ["Src/Common/CQRS/CQRS.Event.Core/CQRS.Event.Core.csproj", "Src/Common/CQRS/CQRS.Event.Core/"]
COPY ["Src/Common/CQRS/CQRS.Core/CQRS.Core.csproj", "Src/Common/CQRS/CQRS.Core/"]
COPY ["Src/Common/CQRS/CQRS.Base/CQRS.Base.csproj", "Src/Common/CQRS/CQRS.Base/"]
COPY ["Src/Common/CQRS/CQRS.Event.Base/CQRS.Event.Base.csproj", "Src/Common/CQRS/CQRS.Event.Base/"]
COPY ["Src/Common/Domain.Support/Domain.Support.csproj", "Src/Common/Domain.Support/"]
COPY ["Src/Common/Repository/Repository.Core/Repository.Core.csproj", "Src/Common/Repository/Repository.Core/"]
COPY ["Src/Common/Repository/Repository.Base/Repository.Base.csproj", "Src/Common/Repository/Repository.Base/"]
COPY ["Src/Core/Infrastructure.Core/Infrastructure.Core.csproj", "Src/Core/Infrastructure.Core/"]
COPY ["Src/Core/Domain.Core/Domain.Core.csproj", "Src/Core/Domain.Core/"]
COPY ["Src/Services/GauTracker/GauTracker.Application/GauTracker.Application.csproj", "Src/Services/GauTracker/GauTracker.Application/"]
COPY ["Src/Services/GauTracker/GauTracker.Domain/GauTracker.Domain.csproj", "Src/Services/GauTracker/GauTracker.Domain/"]
RUN dotnet restore "./Src/Services/GauTracker/GauTracker.API/GauTracker.API.csproj"
COPY . .
WORKDIR "/src/Src/Services/GauTracker/GauTracker.API"
RUN dotnet build "./GauTracker.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GauTracker.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GauTracker.API.dll"]