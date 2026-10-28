# SCF

Sets the carry flag.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Copied from bit 5 of ((Q ^ F) | A). |
| H | Reset. |
| X | Copied from bit 3 of ((Q ^ F) | A). |
| PV | Unaffected. |
| N | Reset. |
| C | Set. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `SCF` | `37` | 4 |  |
| `SCF` | `DD 37` | 8 | Undocumented. |
| `SCF` | `FD 37` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
