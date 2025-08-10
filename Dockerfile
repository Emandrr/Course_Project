# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Course_Project.Web/Course_Project.Web.csproj", "Course_Project.Web/"]
COPY Course_Project.Domain/Course_Project.Domain.csproj Course_Project.Domain/
COPY Course_Project.DataAccess/Course_Project.DataAccess.csproj Course_Project.DataAccess/
COPY Course_Project.Application/Course_Project.Application.csproj Course_Project.Application/
RUN dotnet restore "./Course_Project.Web/Course_Project.Web.csproj"
COPY . .
WORKDIR "/src/Course_Project.Web"
RUN dotnet build "./Course_Project.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Course_Project.WEB.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Course_Project.WEB.dll"]