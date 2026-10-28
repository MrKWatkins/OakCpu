# DEC rr

Decrements rr.

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
| `DEC BC` | `0B` | 6 |  |
| `DEC DE` | `1B` | 6 |  |
| `DEC HL` | `2B` | 6 |  |
| `DEC SP` | `3B` | 6 |  |
| `DEC BC` | `DD 0B` | 10 | Undocumented. |
| `DEC DE` | `DD 1B` | 10 | Undocumented. |
| `DEC IX` | `DD 2B` | 10 |  |
| `DEC SP` | `DD 3B` | 10 | Undocumented. |
| `DEC BC` | `FD 0B` | 10 | Undocumented. |
| `DEC DE` | `FD 1B` | 10 | Undocumented. |
| `DEC IY` | `FD 2B` | 10 |  |
| `DEC SP` | `FD 3B` | 10 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
