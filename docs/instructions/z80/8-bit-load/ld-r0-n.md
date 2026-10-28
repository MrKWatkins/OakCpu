# LD r, n

Loads n into r.

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
| `LD B, n` | `06 n` | 7 |  |
| `LD C, n` | `0E n` | 7 |  |
| `LD D, n` | `16 n` | 7 |  |
| `LD E, n` | `1E n` | 7 |  |
| `LD H, n` | `26 n` | 7 |  |
| `LD L, n` | `2E n` | 7 |  |
| `LD A, n` | `3E n` | 7 |  |
| `LD B, n` | `DD 06 n` | 11 | Undocumented. |
| `LD C, n` | `DD 0E n` | 11 | Undocumented. |
| `LD D, n` | `DD 16 n` | 11 | Undocumented. |
| `LD E, n` | `DD 1E n` | 11 | Undocumented. |
| `LD IXH, n` | `DD 26 n` | 11 | Undocumented. |
| `LD IXL, n` | `DD 2E n` | 11 | Undocumented. |
| `LD A, n` | `DD 3E n` | 11 | Undocumented. |
| `LD B, n` | `FD 06 n` | 11 | Undocumented. |
| `LD C, n` | `FD 0E n` | 11 | Undocumented. |
| `LD D, n` | `FD 16 n` | 11 | Undocumented. |
| `LD E, n` | `FD 1E n` | 11 | Undocumented. |
| `LD IYH, n` | `FD 26 n` | 11 | Undocumented. |
| `LD IYL, n` | `FD 2E n` | 11 | Undocumented. |
| `LD A, n` | `FD 3E n` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
