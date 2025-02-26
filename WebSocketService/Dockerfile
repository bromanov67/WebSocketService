# Используем официальный образ .NET SDK как базовый образ
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем все файлы проекта и публикуем приложение
COPY . ./
RUN dotnet publish -c Release -o out

# Используем официальный образ ASP.NET runtime как базовый образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "WebSocketService.dll"]
