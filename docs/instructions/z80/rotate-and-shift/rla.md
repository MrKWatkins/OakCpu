# RLA

Rotates A left one bit through the carry flag; the carry flag is copied to bit 0 and bit 7 to the carry flag.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Copied from bit 5 of A. |
| H | Reset. |
| X | Copied from bit 3 of A. |
| PV | Unaffected. |
| N | Reset. |
| C | Set if bit 7 of the original value of A is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `RLA` | `17` | 4 |  |
| `RLA` | `DD 17` | 8 | Undocumented. |
| `RLA` | `FD 17` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
