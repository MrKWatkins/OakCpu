# DJNZ d

Decrements B and, if it is not zero, jumps to the address d bytes from the next instruction.

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
| `DJNZ d` | `10 d` | 8 or 13 |  |
| `DJNZ d` | `DD 10 d` | 12 or 17 | Undocumented. |
| `DJNZ d` | `FD 10 d` | 12 or 17 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
