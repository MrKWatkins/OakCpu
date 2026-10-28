# PUSH rr

Pushes rr onto the stack, subtracting two from SP and then storing its high byte at (SP + 1) and its low byte at (SP).

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
| `PUSH BC` | `C5` | 11 |  |
| `PUSH DE` | `D5` | 11 |  |
| `PUSH HL` | `E5` | 11 |  |
| `PUSH AF` | `F5` | 11 |  |
| `PUSH BC` | `DD C5` | 15 | Undocumented. |
| `PUSH DE` | `DD D5` | 15 | Undocumented. |
| `PUSH IX` | `DD E5` | 15 |  |
| `PUSH AF` | `DD F5` | 15 | Undocumented. |
| `PUSH BC` | `FD C5` | 15 | Undocumented. |
| `PUSH DE` | `FD D5` | 15 | Undocumented. |
| `PUSH IY` | `FD E5` | 15 |  |
| `PUSH AF` | `FD F5` | 15 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
