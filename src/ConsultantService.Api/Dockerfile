# ------------------------------------------------------------
# 1) Runtime base image
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ------------------------------------------------------------
# 2) Build stage
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 2.1) Shared Contracts for ProjectReference
COPY ["Common/Contracts/Contracts.Shared/Contracts.Shared.csproj", "Common/Contracts/Contracts.Shared/"]

# 2.2) Consultant-service csproj files
COPY ["Services/Consultant-Service/src/ConsultantService.Api/ConsultantService.Api.csproj", "Services/Consultant-Service/src/ConsultantService.Api/"]
COPY ["Services/Consultant-Service/src/ConsultantService.Application/ConsultantService.Application.csproj", "Services/Consultant-Service/src/ConsultantService.Application/"]
COPY ["Services/Consultant-Service/src/ConsultantService.Domain/ConsultantService.Domain.csproj", "Services/Consultant-Service/src/ConsultantService.Domain/"]
COPY ["Services/Consultant-Service/src/ConsultantService.Infrastructure/ConsultantService.Infrastructure.csproj", "Services/Consultant-Service/src/ConsultantService.Infrastructure/"]

# 2.3) Restore (includes Contracts.Shared)
RUN dotnet restore "Services/Consultant-Service/src/ConsultantService.Api/ConsultantService.Api.csproj"

# 2.4) Copy all source
COPY . .

# 2.5) Publish
WORKDIR "/src/Services/Consultant-Service/src/ConsultantService.Api"
RUN dotnet publish "ConsultantService.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ------------------------------------------------------------
# 3) Final runtime image with wait‐for‐rabbit
# ------------------------------------------------------------
FROM base AS final
WORKDIR /app

# 3.1) Copy the wait‐for‐rabbit script and make it executable
COPY ["scripts/wait-for-rabbit.sh", "/wait-for-rabbit.sh"]
RUN chmod +x /wait-for-rabbit.sh

# 3.2) Copy published output
COPY --from=build /app/publish .

# 3.3) Use the wrapper so the service waits for RabbitMQ
ENTRYPOINT ["/wait-for-rabbit.sh", "rabbitmq", "dotnet", "ConsultantService.Api.dll"]
