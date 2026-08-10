# Secrets

Every credential this solution needs, where it comes from, and how to revoke it.

## The important thing first

The values that used to be in `appsettings*.json` have been blanked, but **that
is not a rotation**. They were committed, some since February 2021, and git
history cannot be un-published — anyone who has ever cloned this repo, or seen
it on a build agent, still has them.

Blanking the files stops the *next* secret from being committed. Only revoking
each value at its provider makes the old one worthless. Until you have worked
through the runbook below, treat every credential in this repo's history as
compromised.

That got more pressing, not less, when the code was mirrored to a public
GitHub repository. The mirror is a snapshot with no history precisely so these
values did not travel — but the infrastructure they unlock is now easy to find
from public source, so the window between "someone reads the code" and
"someone tries the credentials" is shorter than it was.

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

## Rotation runbook

Work down this list. It is ordered by exposure divided by disruption — the
cheap, high-value ones first, the two that need a maintenance window last.

Each value goes into the environment, never back into a file. Locally that is
`dotnet user-secrets`; on the server it is the app pool's environment
variables, or `-e` on the container.

After each one, restart and check the boot log: a `[config]` line means that
key is still missing.

### 1. GreenSms — `GreenSms__Pass`

Do this one first. It sends the SMS login codes, so a leaked password means
someone else's messages on your balance, and sight of the codes themselves.

Change the account password in the GreenSms dashboard, set `GreenSms__Pass`,
restart. Login is broken between those two steps, so it is quick but not
zero-downtime.

### 2. Google reCAPTCHA — `Google__Secret`

Regenerate the secret for the site key in the reCAPTCHA admin console. The
*site* key is public and lives in the web apps; only this one is secret.

Until it is set, `QueryTokenFilter` rejects every public booking with 401 —
the site loads and cannot take a reservation.

### 3. Yandex Object Storage — `YandexStorage__AccessKey`, `YandexStorage__SecretKey`

The only one that rotates with no downtime: create a second static access key
in Yandex Cloud IAM, set both variables, restart, confirm an image upload
works, then delete the old key.

One pair was shared across Local, Development and Production. Issue three.

### 4. Telegram — `BotConfiguration__BotToken`

`/revoke` then `/token` in @BotFather. The bot stops delivering between the
revoke and the restart, and reminder notifications go with it.

Production and development have separate bots and separate tokens; do both.

### 5. Auth0 — `Auth0__UserClientSecret`

Applications → the machine-to-machine app → rotate the client secret.

Only the user-management calls use it, so the blast radius is smaller than it
looks — the mobile and admin login paths do not go through Auth0.

### 6. GeTui — `GeTuiPushOptions__MasterSecret`

Only matters while `PushProvider` is `GeTui`. Once the Capacitor build is what
people are running and the setting flips to `Fcm`, this credential stops being
live and can be retired rather than rotated.

### 7. Postgres — `ConnectionStrings__Postgre`

Needs a window. The same password was used for the production and development
databases, so both connection strings change.

```sql
ALTER USER postgres WITH PASSWORD '…';
```

Then update the variable everywhere the API and the Workers host run, and
restart both. The Workers project reads the API's appsettings, so it needs the
same environment.

### 8. `Secret` — the JWT signing key. Schedule this one.

Everything else on this list is an outage of seconds. This one signs out every
user, because `JwtMiddleware` validates HS256 against it and tokens are issued
with a **ten year** lifetime. The mobile app stores one; re-authenticating
means an SMS to every active user.

Measured against a running API rather than assumed: with the key rotated, a
token issued under the old one gets 401, and one minted under the new key gets
200. There is no grace period.

Generate 64 random bytes; do not reuse anything that has appeared in a chat
log, an issue, or this file:

```bash
node -e "console.log(require('crypto').randomBytes(64).toString('base64url'))"
```

It is still the most important value here — anyone holding it can mint a token
for any owner id and walk past `[AuthorizeOwner]` entirely. Pick a quiet hour,
warn support, and expect the SMS bill.

If the mass logout is unacceptable, the alternative is a code change: accept
two signing keys during a transition window, mint with the new one and validate
against either, then drop the old once the tokens have turned over. That is
real work and nobody has done it.

### Afterwards

The old values stay in the Azure DevOps history — nine commits carry the
production database password alone. Rotation is what makes that history
harmless; there is no need to rewrite it, and rewriting would break every clone
while the values survived in the old ones anyway.

Once this list is done, the history stops being sensitive and can be brought
across to the public GitHub repository, which currently holds a snapshot for
exactly this reason.

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
