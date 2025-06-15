#!/bin/bash

export DOTNET_JitDisasm=Step
export DOTNET_TieredCompilation=0

dotnet run --project MrKWatkins.OakCpu.Z80.Disassemble --configuration Release > Disassembly.txt