# Vegetable.API

Backend for Busy Carrot: the owner-side API, the public booking endpoints, and
the background notification workers.

- `Vegetable.API` — ASP.NET Core 6 web API
- `Vegetable.Core` — repositories, services, EF Core model and migrations
- `Vegetable.Entities` — entity classes
- `Vegetable.Workers` — hosted services for reminders and notification sending
- `Vegetable.UnitTests`

The clients live in [BusyCarrot-V3](https://github.com/OlegMarkov/BusyCarrot-V3):
`apps/admin` (owner desktop), `apps/obs` (public booking site) and `apps/mobile`
(uni-app, packaged with Capacitor).

## This repository is a snapshot

It starts from a single commit rather than carrying the project's history.

That is deliberate. The original history runs to 305 commits from February 2018
and contains live credentials in `appsettings*.json` — the production database
password, object-storage keys, the JWT signing secret, bot tokens — committed
from 2021 onward and not yet rotated. Publishing that history here would have
put all of it on the open internet, where it cannot be recalled.

The full history remains in the original Azure DevOps repository. Once those
credentials have been rotated at their providers the history stops being
sensitive and can be brought across if it is wanted.

## Configuration

No credential is stored in this repository. `appsettings*.json` carries the
shape with empty values; the real ones come from the environment, using a
double underscore for the section separator:

```
ConnectionStrings__Postgre    Secret               GreenSms__Pass
Auth0__UserClientSecret       Google__Secret       BotConfiguration__BotToken
YandexStorage__AccessKey      YandexStorage__SecretKey
```

The API refuses to start without `Secret` and `ConnectionStrings:Postgre`, and
logs a `[config]` line for each optional one that is missing. See
[SECRETS.md](SECRETS.md) for the full list, what each is for, and how to revoke
it.

For local development, `dotnet user-secrets` is read for the `Local`
environment as well as `Development`.

## Running it

Postgres, then either the container:

```bash
docker build -t vegetable-api .
docker run --rm -p 5002:5002 \
  -e ConnectionStrings__Postgre="Host=…;Database=vegetable;Username=…;Password=…" \
  -e Secret="…" \
  vegetable-api
```

or directly:

```bash
dotnet run --project Vegetable.API --launch-profile "Kestrel Local"
```

Migrations apply themselves at startup — `PostgreDbContext` calls
`Database.Migrate()`.

The whole solution targets net6.0 and runs on Linux; the container is the
reference deployment.
