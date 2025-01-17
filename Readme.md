FreshBadge
===

[![Uptime for the past 90 days](https://img.shields.io/endpoint?url=https%3A%2F%2Fwest.aldaviva.com%2Ffreshbadge%2F304333)](https://statuspage.freshping.io/61473-Aldaviva)

Create a [Shields.io](https://shields.io) Badge for a [Freshping](https://freshping.io/) uptime Check, like the one above.

This is a free alternative to the [official Freshping badges](https://support.freshping.io/support/solutions/articles/50000002949-what-is-a-status-badge-), which are theoretically available after reviewing Freshping and [filling out a support request/survey](https://support.freshping.io/support/tickets/new). It's also pretty and uniform looking because it uses Shields.io instead of some ugly one-off design.

<!-- MarkdownTOC autolink="true" bracket="round" autoanchor="false" levels="1,2,3" bullets="-,1." -->

- [Setup](#setup)
    1. [Freshping](#freshping)
    1. [Self-hosted FreshBadge server \(optional\)](#self-hosted-freshbadge-server-optional)
    1. [Shields.io](#shieldsio)
- [API](#api)

<!-- /MarkdownTOC -->

## Setup

### Freshping
1. Log in to your existing [Freshping account](https://login.freshworks.com/email-login).
    - ⛔ [Freshworks disabled the ability to sign up for new Freshping accounts](https://support.freshping.io/en/support/solutions/articles/50000006524-suspension-of-new-signups-faqs) in 2023, so if you don't already have an account, you can't create one anymore.
1. Go to your Freshping Dashboard.
1. Decide which Check you want to appear in your Badge, and go to its Report page.
1. From the Report page URL, copy the Check ID number from the `check_id` query parameter.
1. Add the Check to a Status Page.
    - If this Check is already in at least one Status Page, you can skip this step.
    - If there are no Status Pages in your Freshping account, first create a new Status Page.
    - This step is necessary to allow the Check's status and uptime to be accessed through Freshping's API.

### Self-hosted FreshBadge server (optional)
If you want to use my hosted FreshBadge server instance, skip to the [Shields.io](#shieldsio) steps. Otherwise, you can run a FreshBadge server yourself.

1. Download the [latest release](https://github.com/Aldaviva/FreshBadge/releases/latest) for your operating system and CPU.
1. Extract the ZIP file to a directory on your computer.

#### IIS
1. Ensure the Web Server (IIS) role (`Web-Server`) is installed.
1. Ensure the [ASP.NET Core Hosting Bundle 8 or later](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/hosting-bundle) is installed.
1. Log in to your server using IIS Manager.
1. In Application Pools, create a new Application Pool (CLR 4).
    - Don't use `DefaultAppPool` or any other non-empty pools, because ASP.NET Core webapps must each run isolated in their own pools.
1. In your Site, add a new Application (View Applications › Add Application).
    - The Alias is the URL path prefix (context root) of the app, such as `freshbadge`.
    - Choose the Application Pool you created above.
    - Set the Physical Path to the directory you extracted the FreshBadge release into.

#### Kestrel
1. [Configure](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints) listening ports, TLS certificates, and other settings in `appsettings.json`.
1. Execute the `FreshBadge` program.

### Shields.io
#### Badge JSON URL
```url
https://west.aldaviva.com/freshbadge/{checkId}
```
where `{checkId}` is the ID of the Freshping Check you want to monitor, for example,
```url
https://west.aldaviva.com/freshbadge/304333
```

- For self-hosted FreshBadge instances, replace the origin and path prefix (`https://west.aldaviva.com/freshbadge/`) with your own server's base URL.
- See the [API documentation](#api) for additional parameters that can customize the Badge.

#### Badge SVG URL
```url
https://img.shields.io/endpoint?url={badgeJson}
```
for example
```url
https://img.shields.io/endpoint?url=https%3A%2F%2Fwest.aldaviva.com%2Ffreshbadge%2F304333
```

- You can customize the Badge appearance using the [Endpoint Badge query parameters](https://shields.io/badges/endpoint-badge#:~:text=the%20query%20string.-,Query%20Parameters,-url%20string%20%E2%80%94).
    ```url
    https://img.shields.io/endpoint?url=https%3A%2F%2Fwest.aldaviva.com%2Ffreshbadge%2F304333&label=uptime+(90+days)
    ```

#### SVG image in Markdown
```markdown
![Uptime for the past 90 days](https://img.shields.io/endpoint?url=https%3A%2F%2Fwest.aldaviva.com%2Ffreshbadge%2F304333)
```
![Uptime for the past 90 days](https://img.shields.io/endpoint?url=https%3A%2F%2Fwest.aldaviva.com%2Ffreshbadge%2F304333)

#### Link to Status Page in Markdown
```markdown
[![Uptime for the past 90 days](https://img.shields.io/endpoint?url=https%3A%2F%2Fwest.aldaviva.com%2Ffreshbadge%2F304333)](https://statuspage.freshping.io/61473-Aldaviva)
```
[![Uptime for the past 90 days](https://img.shields.io/endpoint?url=https%3A%2F%2Fwest.aldaviva.com%2Ffreshbadge%2F304333)](https://statuspage.freshping.io/61473-Aldaviva)

## API

- URL template: `https://west.aldaviva.com/freshbadge/{checkId}?period={period}`
- Verb: `GET`
- Parameters
    - `checkId`
        - **importance:** required
        - **location:** path parameter
        - **type:** number (64-bit signed integer)
        - **meaning:** numeric ID of the Freshping Check to show uptime for, as seen in the `check_id` query parameter of the Freshping report page for this Check
    - `period`
        - **importance:** optional
        - **location:** query parameter
        - **type:** string
        - **format:** [ISO 8601 time period](https://en.wikipedia.org/wiki/ISO_8601#Durations)
        - **meaning:** period over which to calculate the Check's uptime percentage, ending at the current time
        - **validity:** in the range (0, 90 days]
        - **default:** 90 days
        - **example:** `?period=P30D` (30 days) ![30 days](https://img.shields.io/endpoint?url=https%3A%2F%2Fwest.aldaviva.com%2Ffreshbadge%2F304333%3Fperiod%3DP30D)
    - `precision`
        - **importance:** optional
        - **location:** query parameter
        - **type:** number (8-bit unsigned integer)
        - **meaning:** how many digits after the decimal point the uptime percentage should show
        - **validity:** in the range [0, 256)
        - **default:** 7 (for the fabled "9 nines," because 99.9999999% has 7 nines after the decimal point)
        - **example:** `?precision=2` (2 digits after the decimal point: `99.99%`) ![2 digits](https://img.shields.io/endpoint?url=https%3A%2F%2Fwest.aldaviva.com%2Ffreshbadge%2F304333%3Fprecision%3D2)
    - `locale` 
        - **importance:** optional
        - **location:** query parameter
        - **type:** string
        - **format:** [IETF BCP 47 language tag](https://en.wikipedia.org/wiki/IETF_language_tag)
        - **meaning:** locale to use when rendering the uptime label and percentage
        - **validity:** any language tag supported by [Windows](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c) or [ICU](https://icu.unicode.org/) (on Unix); the "uptime" label is currently badly localized in `de`, `en`, `es`, and `fr`.
        - **default:** `en-US` (US English), or the server user's locale when self-hosted
        - **example:** `?locale=fr` (France format: `99,99 %`) ![2 digits](https://img.shields.io/endpoint?url=https%3A%2F%2Fwest.aldaviva.com%2Ffreshbadge%2F304333%3Flocale%3Dfr)
- Response body: JSON object that conforms to the [Shields.io JSON Endpoint schema](https://shields.io/badges/endpoint-badge#:~:text=Example%20Shields%20Response-,Schema,-Property)
    ```json
    {
        "schemaVersion": 1,
        "label": "uptime",
        "message": "99.9132844%",
        "color": "success",
        "isError": false,
        "logoSvg": "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 32 32\"><path fill=\"#fff\" d=\"M28 0H16C7.2 0 0 7.2 0 16s7.2 16 16 16 16-7.2 16-16V4c0-2.2-1.8-4-4-4zM16 7.7c4.4 0 8 3.5 8.3 7.8h-4l-2.4-3.1c-.2-.3-.6-.4-1-.4-.4.1-.7.3-.8.7l-1.8 5.1-1.3-1.9c-.2-.3-.5-.4-.8-.4H7.7c.2-4.4 3.9-7.8 8.3-7.8zm0 16.6c-4.1 0-7.5-2.9-8.2-6.8h3.9l2.3 3c.2.3.5.4.8.4h.2c.4-.1.7-.3.8-.7l1.8-5.1 1.5 2c.2.2.5.4.8.4h4.4c-.8 3.9-4.2 6.8-8.3 6.8z\"/></svg>"
    }
    ```