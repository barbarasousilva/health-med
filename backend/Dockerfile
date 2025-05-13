FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY HealthMed.sln .

COPY HealthMed.Api/ ./HealthMed.Api/
COPY HealthMed.Application/ ./HealthMed.Application/
COPY HealthMed.Domain/ ./HealthMed.Domain/
COPY HealthMed.Infrastructure/ ./HealthMed.Infrastructure/
COPY HealthMed.Tests.Integration/ ./HealthMed.Tests.Integration/
COPY HealthMed.Tests.Unit/ ./HealthMed.Tests.Unit/

RUN dotnet restore
RUN dotnet publish HealthMed.Api/HealthMed.Api.csproj -c Release -o /out
RUN dotnet publish HealthMed.Application/HealthMed.Application.csproj -c Release -o /out
RUN dotnet publish HealthMed.Domain/HealthMed.Domain.csproj -c Release -o /out
RUN dotnet publish HealthMed.Infrastructure/HealthMed.Infrastructure.csproj -c Release -o /out
RUN dotnet publish HealthMed.Tests.Integration/HealthMed.Tests.Integration.csproj -c Release -o /out
RUN dotnet publish HealthMed.Tests.Unit/HealthMed.Tests.Unit.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "HealthMed.Api.dll"]