# ���������� ����������� ����� .NET SDK ��� ������� �����
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# �������� csproj � ��������������� �����������
COPY *.csproj ./
RUN dotnet restore

# �������� ��� ����� ������� � ��������� ����������
COPY . ./
RUN dotnet publish -c Release -o out

# ���������� ����������� ����� ASP.NET runtime ��� ������� ����� ��� �������
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "WebSocketService.dll"]
