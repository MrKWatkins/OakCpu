# LD A, (rr)

Loads the byte at (rr) into A.

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
| `LD A, (BC)` | `0A` | 7 |  |
| `LD A, (DE)` | `1A` | 7 |  |
| `LD A, (BC)` | `DD 0A` | 11 | Undocumented. |
| `LD A, (DE)` | `DD 1A` | 11 | Undocumented. |
| `LD A, (BC)` | `FD 0A` | 11 | Undocumented. |
| `LD A, (DE)` | `FD 1A` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
