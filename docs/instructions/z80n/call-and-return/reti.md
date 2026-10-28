# RETI

Returns from a maskable interrupt handler, popping the return address from the stack into PC and copying IFF2 to IFF1. Z80 peripherals recognise the instruction as the end of the interrupt.

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
| `RETI` | `ED 4D` | 14 |  |
| `RETI` | `ED 5D` | 14 | Undocumented. |
| `RETI` | `ED 6D` | 14 | Undocumented. |
| `RETI` | `ED 7D` | 14 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
