# RETN

Returns from a non-maskable interrupt handler, popping the return address from the stack into PC and copying IFF2 to IFF1 to restore the interrupt state from before the interrupt.

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
| `RETN` | `ED 45` | 14 |  |
| `RETN` | `ED 55` | 14 | Undocumented. |
| `RETN` | `ED 65` | 14 | Undocumented. |
| `RETN` | `ED 75` | 14 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
