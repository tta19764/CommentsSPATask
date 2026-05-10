FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends fontconfig fonts-dejavu-core \
    && mkdir -p /app/uploads \
    && chown -R $APP_UID /app \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CommentsSPATask.Api/CommentsSPATask.Api.csproj", "CommentsSPATask.Api/"]
RUN dotnet restore "CommentsSPATask.Api/CommentsSPATask.Api.csproj"
COPY . .
WORKDIR "/src/CommentsSPATask.Api"
RUN dotnet build "./CommentsSPATask.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CommentsSPATask.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommentsSPATask.Api.dll"]
