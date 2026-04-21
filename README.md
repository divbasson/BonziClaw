# BonziClaw

A modern, spyware-free reinterpretation of BonziBuddy for Windows, designed as a desktop face/front-end for OpenClaw.

A classic desktop-assistant experience is reimagined here with a privacy-first approach. The project keeps the familiar animated character interaction while avoiding bundled adware/spyware behavior.

This project intentionally avoids that model. BonzoBuddo is focused on transparent local control: Bonzi acts as the animated UI persona, while OpenClaw handles conversational intelligence.

## What This App Does

- Runs as a Windows tray app with quick actions.
- Uses Double Agent/MS Agent to animate and speak as Bonzi.
- Routes chat requests to OpenClaw.
- Supports Telegram bot integration that also routes through OpenClaw.
- Loads local configuration from `%APPDATA%/BonzoBuddo/appconfig.json` and environment variables.

## Core Features

- Ask AI UI flow: user prompt -> OpenClaw -> response -> Bonzi speech/animation.
- Telegram flow: Telegram message -> OpenClaw -> Telegram reply + Bonzi speech callback.
- Tray icon uses `ai-icon.svg`.
- OpenClaw connectivity supports WebSocket-first with HTTP fallback.
- Local `.env` loading for secrets and private endpoints.

## Tech Stack

- .NET 6 WinForms (`net6.0-windows`)
- x86 target (`PlatformTarget=x86`)
- ActiveX interop (`AxAgentCtl`, `AgentObjects`)
- Double Agent / MS Agent character runtime
- `Svg` package for rendering the tray icon from SVG

## Quick Start

1. Install prerequisites:
- Windows machine
- .NET SDK 6+ installed
- Double Agent runtime and required agent assets

2. Clone and restore:

```powershell
git clone <your-repo-url>
cd BonziBuddy
dotnet restore .\BonzoBuddo.sln
```

3. Create local environment file:

```powershell
copy .env.example .env
```

4. Fill `.env` values (example keys):
- `BONZO_OPENCLAW_URL`
- `BONZO_OPENCLAW_TOKEN`
- `BONZO_OPENCLAW_PASSWORD`
- `BONZO_OPENCLAW_MODEL`
- `BONZO_TELEGRAM_BOT_TOKEN`
- `BONZO_TELEGRAM_ENABLED`

5. Build and run:

```powershell
dotnet build .\BonzoBuddo.sln -c Debug
dotnet run --project .\BonzoBuddo\BonzoBuddo.csproj
```

## Configuration and Secrets

- Do not commit `.env`.
- `.env` is ignored by `.gitignore`.
- Use `.env.example` as your template.
- Runtime config is also persisted to `%APPDATA%/BonzoBuddo/appconfig.json`.
- If a real token has ever been exposed in commits, rotate it immediately.

## Project Layout

- `BonzoBuddo/` main WinForms application
- `BonzoBuddo/Helpers/` OpenClaw, Telegram, config, utility helpers
- `BonzoBuddo/Forms/` UI forms including Ask AI and OpenClaw config
- `ai-icon.svg` tray icon source
- `.github/workflows/` CI pipelines

## Build Pipeline

GitHub Actions workflow builds on Windows for push and pull requests:

- Restores NuGet packages
- Builds `BonzoBuddo.sln` in Release configuration

See `.github/workflows/build.yml`.

## Disclaimer

This repository is an independent community project and is provided for educational and nostalgic software preservation purposes.

## Attribution

Michaél Landry for his bonzo code.

## Trivy Scan:

Report Summary
┌──────────────────────────────────────────────────────────┬─────────────┬─────────────────┬─────────┐
│                          Target                          │    Type     │ Vulnerabilities │ Secrets │
├──────────────────────────────────────────────────────────┼─────────────┼─────────────────┼─────────┤
│ BonzoBuddo/bin/Debug/net6.0-windows/BonzoBuddo.deps.json │ dotnet-core │        0        │    -    │
└──────────────────────────────────────────────────────────┴─────────────┴─────────────────┴─────────┘
Legend:
- '-': Not scanned
- '0': Clean (no security findings detected)
