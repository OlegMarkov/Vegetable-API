# Secrets

Every credential this solution needs, where it comes from, and how to revoke it.

## The important thing first

The values that used to be in `appsettings*.json` have been blanked, but **that
is not a rotation**. They were committed, some since February 2021, and git
history cannot be un-published — anyone who has ever cloned this repo, or seen
it on a build agent, still has them.

Blanking the files stops the *next* secret from being committed. Only revoking
each value at its provider makes the old one worthless. Until you do the table
below, treat every credential in this repo's history as compromised.

Rewriting history is not the fix and is not recommended here: it would break
every clone of a shared repository, and the values would still exist in old
clones, forks, build caches and anyone's disk. Revoke instead.

## How configuration is supplied now

ASP.NET Core's default chain already reads environment variables, so nothing in
the code changed for values that go through `IConfiguration`. Use a double
underscore for the section separator:

| Config key | Environment variable |
|---|---|
| `Secret` | `Secret` |
| `ConnectionStrings:Postgre` | `ConnectionStrings__Postgre` |
| `Auth0:UserClientSecret` | `Auth0__UserClientSecret` |
| `GreenSms:Pass` | `GreenSms__Pass` |
| `YandexStorage:AccessKey` | `YandexStorage__AccessKey` |
| `YandexStorage:SecretKey` | `YandexStorage__SecretKey` |
| `BotConfiguration:BotToken` | `BotConfiguration__BotToken` |
| `Google:Secret` | `Google__Secret` |
| `GeTuiPushOptions:AppKey` | `GeTuiPushOptions__AppKey` |
| `GeTuiPushOptions:MasterSecret` | `GeTuiPushOptions__MasterSecret` |
| `Payment:TerminalKey` | `Payment__TerminalKey` |
| `Payment:TerminalPassword` | `Payment__TerminalPassword` |

On IIS, set them on the application pool, or in the `<environmentVariables>`
block of a `web.config` that is generated at deploy time and not committed.
For local development, `dotnet user-secrets` keeps them off disk in the repo.

The API refuses to start without `Secret` and `ConnectionStrings:Postgre`, and
logs a `[config]` line at boot for each missing optional one. That check is
`Vegetable.API/Configuration/RequiredSecrets.cs`.

## What to revoke, and where

| Credential | Provider | Notes |
|---|---|---|
| `Secret` (JWT signing) | ours — just generate one | **See the warning below.** 64+ random chars. |
| `ConnectionStrings:Postgre` | Postgres on 84.201.169.106 | `ALTER USER postgres WITH PASSWORD ...`. The same password was used for prod and dev. |
| `Auth0:UserClientSecret` | Auth0 dashboard | Applications → the machine-to-machine app → Rotate. |
| `GreenSms:Pass` | GreenSms account | **Prioritise this.** It sends the SMS login codes; an attacker can burn the balance or harvest codes. |
| `YandexStorage:AccessKey` / `SecretKey` | Yandex Cloud IAM | Delete the static access key, make a new one. One key pair was shared across Local, Development *and* Production. |
| `BotConfiguration:BotToken` | Telegram @BotFather | `/revoke` then `/token`. Separate tokens for prod and dev. |
| `Google:Secret` | Google reCAPTCHA admin | Server-side secret for the site key. The *site* key is public and lives in the web apps; only this one is secret. |
| `GeTuiPushOptions:MasterSecret` | GeTui console | Only matters while `PushProvider` is `GeTui`. |
| `Payment:TerminalKey` / `TerminalPassword` | Tinkoff merchant portal | The committed values were the public demo terminal, but rotate whatever production uses. |

## Rotating `Secret` logs everyone out

`Secret` signs the auth tokens. `JwtMiddleware` validates against it and
`AuthenticationService` mints with it, and those tokens are issued with a **ten
year** lifetime, which the mobile app stores and reuses.

Change it and every token ever issued stops validating. Every user — mobile and
admin — is signed out and has to log in again, which for mobile means receiving
an SMS code. That is a support event, not a deploy detail: plan when it happens
and expect the SMS bill.

It is still the most important one to rotate, because anyone holding it can mint
a valid token for any owner id and walk straight past `[AuthorizeOwner]`. There
is no way to have that both ways; pick a time.

If you want to avoid the mass logout, the alternative is a code change:
accept two signing keys during a transition window, minting with the new one
and validating against either, then drop the old key once the tokens have
turned over. That is real work and nobody has done it here.

## Two test back doors, now closed by default

Both were live in production.

`SecretCaptcha` / `SecretPhone` skipped captcha validation. The comparison was
`captcha == _configuration["SecretCaptcha"]` — with the setting absent that read
null, and `captcha` is a query parameter that is also null when omitted, so
`null == null` matched and **captcha validation was skipped entirely for any
request that just left the parameter off**. It is now guarded on the setting
being present and non-empty, so leaving the key out disables the door instead of
removing the lock.

`AllowTestVerificationCode` gates the other one: any phone number containing
`123456` got the verification code `123456`. That was unconditional. It is now
off unless asked for, and only `appsettings.Local.json` and
`appsettings.Development.json` ask.

## Not a secret, despite appearances

- `apps/mobile/android/app/google-services.json` — public, ships inside the APK.
- The reCAPTCHA **site** key (`VITE_RECAPTCHA_SITE_KEY`) — public by design.
- `GeTuiPushOptions:AppID`, `Auth0:UserClientId`, `YandexStorage:BucketName` —
  identifiers, not credentials.

## Genuinely secret, and gitignored

- `firebase-service-account.json` — can send push as us.
- The Android release keystore — cannot be rotated at all. Lose control of it
  and anyone can publish an update as us; lose the file and the Play listing can
  never be updated again.
