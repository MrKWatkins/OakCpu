# SRA r

Shifts r right one bit; bit 0 is copied to the carry flag and bit 7 keeps its value, preserving the sign.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of r. |
| Z | Set if r is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Reset. |
| X | Copied from bit 3 of r. |
| PV | Set if r has even parity, otherwise reset. |
| N | Reset. |
| C | Copied from bit 0 of the original value of r. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `SRA B` | `CB 28` | 8 |  |
| `SRA C` | `CB 29` | 8 |  |
| `SRA D` | `CB 2A` | 8 |  |
| `SRA E` | `CB 2B` | 8 |  |
| `SRA H` | `CB 2C` | 8 |  |
| `SRA L` | `CB 2D` | 8 |  |
| `SRA A` | `CB 2F` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
