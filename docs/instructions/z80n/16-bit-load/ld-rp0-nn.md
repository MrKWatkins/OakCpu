# LD rr, nn

Loads nn into rr.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Unaffected. |
| H | Unaffected. |
| X | Unaffected. |
| PV | Unaffected. |
| N | Unaffected. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `LD BC, nn` | `01 nn` | 10 |  |
| `LD DE, nn` | `11 nn` | 10 |  |
| `LD HL, nn` | `21 nn` | 10 |  |
| `LD SP, nn` | `31 nn` | 10 |  |
| `LD BC, nn` | `DD 01 nn` | 14 | Undocumented. |
| `LD DE, nn` | `DD 11 nn` | 14 | Undocumented. |
| `LD IX, nn` | `DD 21 nn` | 14 |  |
| `LD SP, nn` | `DD 31 nn` | 14 | Undocumented. |
| `LD BC, nn` | `FD 01 nn` | 14 | Undocumented. |
| `LD DE, nn` | `FD 11 nn` | 14 | Undocumented. |
| `LD IY, nn` | `FD 21 nn` | 14 |  |
| `LD SP, nn` | `FD 31 nn` | 14 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
