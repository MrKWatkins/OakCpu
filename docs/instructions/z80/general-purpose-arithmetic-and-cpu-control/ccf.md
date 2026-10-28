# CCF

Inverts the carry flag.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Copied from bit 5 of ((Q ^ F) | A). |
| H | Set to the previous value of the C flag. |
| X | Copied from bit 3 of ((Q ^ F) | A). |
| PV | Unaffected. |
| N | Reset. |
| C | Inverted. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `CCF` | `3F` | 4 |  |
| `CCF` | `DD 3F` | 8 | Undocumented. |
| `CCF` | `FD 3F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
