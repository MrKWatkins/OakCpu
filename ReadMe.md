# OakCpu

[![Build Status](https://github.com/MrKWatkins/OakCpu/actions/workflows/build.yml/badge.svg)](https://github.com/MrKWatkins/OakCpu/actions/workflows/build.yml)
[![Z80 NuGet Version](https://img.shields.io/nuget/v/MrKWatkins.OakCpu.Z80?label=Z80%20Nuget%20Version)](https://www.nuget.org/packages/MrKWatkins.OakCpu.Z80)
[![Z80 NuGet Downloads](https://img.shields.io/nuget/dt/MrKWatkins.OakCpu.Z80?label=Z80%20Nuget%20Downloads)](https://www.nuget.org/packages/MrKWatkins.OakCpu.Z80)
[![6502 NuGet Version](https://img.shields.io/nuget/v/MrKWatkins.OakCpu.M6502?label=6502%20Nuget%20Version)](https://www.nuget.org/packages/MrKWatkins.OakCpu.M6502)
[![6502 NuGet Downloads](https://img.shields.io/nuget/dt/MrKWatkins.OakCpu.M6502?label=6502%20Nuget%20Downloads)](https://www.nuget.org/packages/MrKWatkins.OakCpu.M6502)

> Emulators for 8-bit systems.

Full documentation is available at [mrkwatkins.github.io/OakCpu](https://mrkwatkins.github.io/OakCpu/). Source code and issue tracking are on [GitHub](https://github.com/MrKWatkins/OakCpu).

## Licencing

Licensed under GPL v3.0.

## Building and documentation

Build and Test compiles the solution, runs the tests and validates the complete documentation on pushes and pull requests. Release runs the same validation,
then publishes the prepared packages and documentation. Leave the version blank for the next minor release, or enter an explicit stable version.
Both publication options default to true. Turn **publish-packages** off to update only documentation without creating a new package or release tag.
The API reference then uses the latest released source; `api-ref` can select corrected XML comments from another commit.

Preview with `python3 docs/preview.py` or **Preview documentation** in a JetBrains IDE, with Docker and the .NET 10 SDK installed.
Open `http://localhost:5173/OakCpu/`. Generated API pages and site output are ignored build artifacts.

Configure NuGet trusted publishing for repository `OakCpu`, workflow `release.yml`, environment `github-pages`, owner `MrKWatkins` and the three CPU packages.
Set GitHub Pages to **GitHub Actions**, and allow `main` to deploy to the `github-pages` environment. No stored NuGet API key is required.
