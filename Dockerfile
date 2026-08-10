# Vegetable.API on Linux.
#
# The whole solution targets net6.0, so this is a stock two-stage .NET build
# with nothing platform-specific in it. Vegetable.Entities was the last project
# pinned to .NET Framework 4.7.2 and was retargeted; there is no Windows-only
# dependency left. The captcha library draws with SixLabors.ImageSharp rather
# than System.Drawing, and DateTimeExtensions already falls back from Windows
# timezone ids to IANA ones, which is what Linux understands.
#
#   docker build -t vegetable-api .
#   docker run --rm -p 5002:5002 \
#     -e ConnectionStrings__Postgre="Host=...;Database=vegetable;Username=...;Password=..." \
#     -e Secret="..." \
#     vegetable-api
#
# Every secret comes from the environment; none are baked into the image. See
# SECRETS.md for the full list and the double-underscore convention.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Restore against the project files alone first, so a change to source code does
# not invalidate the (slow) restore layer.
COPY Vegetable.API/Vegetable.API.csproj        Vegetable.API/
COPY Vegetable.Core/Vegetable.Core.csproj      Vegetable.Core/
COPY Vegetable.Entities/Vegetable.Entities.csproj Vegetable.Entities/
RUN dotnet restore Vegetable.API/Vegetable.API.csproj

# UnitTests and Workers are deliberately not copied: this image is the API. The
# Workers host is a separate process and wants its own image.
COPY Vegetable.API/      Vegetable.API/
COPY Vegetable.Core/     Vegetable.Core/
COPY Vegetable.Entities/ Vegetable.Entities/

RUN dotnet publish Vegetable.API/Vegetable.API.csproj \
      -c Release \
      -o /app \
      --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# 0.0.0.0, not localhost. Binding the loopback interface inside a container
# leaves the process healthy and unreachable, with nothing in any log to say so.
#
# Both names are set on purpose. Every appsettings.*.json in this repo pins a
# "Urls" key to localhost — Production included, and appsettings.Local.json
# pins it to *https*, which in a container fails outright with "no server
# certificate was specified". That key lives in app configuration and wins over
# ASPNETCORE_URLS, so setting only the latter is not enough. Environment
# variables are layered after appsettings, so setting "Urls" here beats all of
# them whichever ASPNETCORE_ENVIRONMENT the container runs as.
ENV ASPNETCORE_URLS=http://0.0.0.0:5002
ENV Urls=http://0.0.0.0:5002
EXPOSE 5002

COPY --from=build /app .

# Run as a non-root user. The .NET 6 aspnet image does not ship one — $APP_UID
# only exists from .NET 8 — so create it explicitly rather than defaulting to
# root. The app writes nothing to disk that it needs to own.
RUN useradd --uid 64198 --create-home --shell /usr/sbin/nologin app \
    && chown -R app:app /app
USER app

ENTRYPOINT ["dotnet", "Vegetable.API.dll"]
