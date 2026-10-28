# LD rr, (nn)

Loads the word at address nn into rr, low byte first.

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
| `LD HL, (nn)` | `2A nn` | 16 |  |
| `LD IX, (nn)` | `DD 2A nn` | 20 |  |
| `LD BC, (nn)` | `ED 4B nn` | 20 |  |
| `LD DE, (nn)` | `ED 5B nn` | 20 |  |
| `LD HL, (nn)` | `ED 6B nn` | 20 | Undocumented. |
| `LD SP, (nn)` | `ED 7B nn` | 20 |  |
| `LD IY, (nn)` | `FD 2A nn` | 20 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
