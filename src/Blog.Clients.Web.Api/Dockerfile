# FRONTEND

FROM node:24-bookworm-slim AS frontend

WORKDIR /src

COPY src/Blog.Clients.Web.UI/package*.json \
     src/Blog.Clients.Web.UI/

WORKDIR /src/src/Blog.Clients.Web.UI

RUN npm ci

COPY src/Blog.Clients.Web.UI/ .

RUN npm run build

# BACKEND

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION

WORKDIR /src

COPY ["src/Blog.Clients.Web.Api/Blog.Clients.Web.Api.csproj", "src/Blog.Clients.Web.Api/"]
COPY ["src/Blog.Application/Blog.Application.csproj", "src/Blog.Application/"]
COPY ["src/Blog.Domain/Blog.Domain.csproj", "src/Blog.Domain/"]
COPY ["src/Blog.Infrastructure.DatabaseMigrations/Blog.Infrastructure.DatabaseMigrations.csproj", "src/Blog.Infrastructure.DatabaseMigrations/"]
COPY ["src/Blog.Infrastructure/Blog.Infrastructure.csproj", "src/Blog.Infrastructure/"]
COPY ["Directory.Build.props", "/"]
COPY ["Directory.Packages.props", "/"]

RUN dotnet restore "./src/Blog.Clients.Web.Api/Blog.Clients.Web.Api.csproj"

COPY . .

WORKDIR /src/src/Blog.Clients.Web.Api

RUN dotnet publish "./Blog.Clients.Web.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

COPY --from=frontend \
    /src/src/Blog.Clients.Web.UI/dist/blog.client \
    /app/publish/wwwroot/

# RUNTIME

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Blog.Clients.Web.Api.dll"]