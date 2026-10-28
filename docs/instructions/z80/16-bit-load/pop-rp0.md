# POP rr

Pops rr from the stack, loading its low byte from (SP) and its high byte from (SP + 1) and then adding two to SP.

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
| `POP BC` | `C1` | 10 |  |
| `POP DE` | `D1` | 10 |  |
| `POP HL` | `E1` | 10 |  |
| `POP AF` | `F1` | 10 |  |
| `POP BC` | `DD C1` | 14 | Undocumented. |
| `POP DE` | `DD D1` | 14 | Undocumented. |
| `POP IX` | `DD E1` | 14 |  |
| `POP AF` | `DD F1` | 14 | Undocumented. |
| `POP BC` | `FD C1` | 14 | Undocumented. |
| `POP DE` | `FD D1` | 14 | Undocumented. |
| `POP IY` | `FD E1` | 14 |  |
| `POP AF` | `FD F1` | 14 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
