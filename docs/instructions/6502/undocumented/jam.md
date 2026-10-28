# JAM

Jams the CPU: it reads from 0xFFFF and 0xFFFE and then does nothing until it is reset.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Unaffected. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Unaffected. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `JAM` | `02` | 11 |  |
| `JAM` | `12` | 11 |  |
| `JAM` | `22` | 11 |  |
| `JAM` | `32` | 11 |  |
| `JAM` | `42` | 11 |  |
| `JAM` | `52` | 11 |  |
| `JAM` | `62` | 11 |  |
| `JAM` | `72` | 11 |  |
| `JAM` | `92` | 11 |  |
| `JAM` | `B2` | 11 |  |
| `JAM` | `D2` | 11 |  |
| `JAM` | `F2` | 11 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
