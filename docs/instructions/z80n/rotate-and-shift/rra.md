# RRA

Rotates A right one bit through the carry flag; the carry flag is copied to bit 7 and bit 0 to the carry flag.

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
| C | Set if bit 0 of the original value of A is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `RRA` | `1F` | 4 |  |
| `RRA` | `DD 1F` | 8 | Undocumented. |
| `RRA` | `FD 1F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
