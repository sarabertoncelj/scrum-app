FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src

EXPOSE 80
EXPOSE 443

# copy csproj, dependencies
COPY ["smrpo-be/smrpo-be.csproj", "smrpo-be/"]
RUN dotnet restore "smrpo-be/smrpo-be.csproj"

# copy everything else and build
COPY . .

WORKDIR "/src/smrpo-be"
RUN dotnet publish "smrpo-be.csproj" -c Release -o /app --no-restore

# final
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
COPY --from=build /app ./
COPY smrpo-be/https/* /app/
RUN chmod 0444 /app/localhost.pfx
ENTRYPOINT ["dotnet", "smrpo-be.dll"]
