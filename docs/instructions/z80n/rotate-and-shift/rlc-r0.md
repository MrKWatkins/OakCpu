# RLC r

Rotates r left one bit; bit 7 is copied to both bit 0 and the carry flag.

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
| C | Set if bit 7 of the original value of r is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `RLC B` | `CB 00` | 8 |  |
| `RLC C` | `CB 01` | 8 |  |
| `RLC D` | `CB 02` | 8 |  |
| `RLC E` | `CB 03` | 8 |  |
| `RLC H` | `CB 04` | 8 |  |
| `RLC L` | `CB 05` | 8 |  |
| `RLC A` | `CB 07` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
