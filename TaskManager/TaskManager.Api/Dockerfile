FROM mcr.microsoft.com/dotnet/sdk:9.0

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

EXPOSE 8080

WORKDIR /src
COPY ["TaskManager.Api/TaskManager.Api.csproj", "TaskManager.Api/"]
COPY ["TaskManager.Common/TaskManager.Common.csproj", "TaskManager.Common/"]
RUN dotnet restore "TaskManager.Api/TaskManager.Api.csproj"
COPY . .
RUN dotnet publish "TaskManager.Api/TaskManager.Api.csproj" -c Release -o /app/api /p:UseAppHost=false

ENTRYPOINT ["./TaskManager.Api/entrypoint.sh"]