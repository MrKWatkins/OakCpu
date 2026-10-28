# RET cc

If condition cc is met, pops the return address from the stack into PC; otherwise continues with the next instruction.

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
| `RET NZ` | `C0` | 5 or 11 |  |
| `RET Z` | `C8` | 5 or 11 |  |
| `RET NC` | `D0` | 5 or 11 |  |
| `RET C` | `D8` | 5 or 11 |  |
| `RET PO` | `E0` | 5 or 11 |  |
| `RET PE` | `E8` | 5 or 11 |  |
| `RET P` | `F0` | 5 or 11 |  |
| `RET M` | `F8` | 5 or 11 |  |
| `RET NZ` | `DD C0` | 9 or 15 | Undocumented. |
| `RET Z` | `DD C8` | 9 or 15 | Undocumented. |
| `RET NC` | `DD D0` | 9 or 15 | Undocumented. |
| `RET C` | `DD D8` | 9 or 15 | Undocumented. |
| `RET PO` | `DD E0` | 9 or 15 | Undocumented. |
| `RET PE` | `DD E8` | 9 or 15 | Undocumented. |
| `RET P` | `DD F0` | 9 or 15 | Undocumented. |
| `RET M` | `DD F8` | 9 or 15 | Undocumented. |
| `RET NZ` | `FD C0` | 9 or 15 | Undocumented. |
| `RET Z` | `FD C8` | 9 or 15 | Undocumented. |
| `RET NC` | `FD D0` | 9 or 15 | Undocumented. |
| `RET C` | `FD D8` | 9 or 15 | Undocumented. |
| `RET PO` | `FD E0` | 9 or 15 | Undocumented. |
| `RET PE` | `FD E8` | 9 or 15 | Undocumented. |
| `RET P` | `FD F0` | 9 or 15 | Undocumented. |
| `RET M` | `FD F8` | 9 or 15 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
