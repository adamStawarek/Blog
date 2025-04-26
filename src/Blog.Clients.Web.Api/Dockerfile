# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION
WORKDIR /src

# Install Node.js 18
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

COPY ["src/Blog.Clients.Web.UI/nuget.config", "src/Blog.Clients.Web.UI/"]
COPY ["src/Blog.Clients.Web.Api/Blog.Clients.Web.Api.csproj", "src/Blog.Clients.Web.Api/"]
COPY ["src/Blog.Application/Blog.Application.csproj", "src/Blog.Application/"]
COPY ["src/Blog.Domain/Blog.Domain.csproj", "src/Blog.Domain/"]
COPY ["src/Blog.Clients.Web.UI/Blog.Clients.Web.UI.esproj", "src/Blog.Clients.Web.UI/"]
COPY ["src/Blog.Infrastructure.DatabaseMigrations/Blog.Infrastructure.DatabaseMigrations.csproj", "src/Blog.Infrastructure.DatabaseMigrations/"]
COPY ["src/Blog.Infrastructure/Blog.Infrastructure.csproj", "src/Blog.Infrastructure/"]
COPY ["Directory.Build.props", "/"]
COPY ["Directory.Packages.props", "/"]
RUN dotnet restore "./src/Blog.Clients.Web.Api/Blog.Clients.Web.Api.csproj"
COPY . .
WORKDIR "/src/src/Blog.Clients.Web.Api"
RUN dotnet build "./Blog.Clients.Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./Blog.Clients.Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blog.Clients.Web.Api.dll"]