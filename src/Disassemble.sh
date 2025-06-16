#!/bin/bash

export DOTNET_JitDisasm=Flags_237
export DOTNET_TieredCompilation=0

dotnet run --project MrKWatkins.OakCpu.Z80.Disassemble --configuration Release > Disassembly.txt