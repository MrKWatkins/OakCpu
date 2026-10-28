# SRL r

Shifts r right one bit; bit 0 is copied to the carry flag and bit 7 is reset.

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
| `SRL B` | `CB 38` | 8 |  |
| `SRL C` | `CB 39` | 8 |  |
| `SRL D` | `CB 3A` | 8 |  |
| `SRL E` | `CB 3B` | 8 |  |
| `SRL H` | `CB 3C` | 8 |  |
| `SRL L` | `CB 3D` | 8 |  |
| `SRL A` | `CB 3F` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
