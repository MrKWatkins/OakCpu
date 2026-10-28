# RR r

Rotates r right one bit through the carry flag; the carry flag is copied to bit 7 and bit 0 to the carry flag.

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
| `RR B` | `CB 18` | 8 |  |
| `RR C` | `CB 19` | 8 |  |
| `RR D` | `CB 1A` | 8 |  |
| `RR E` | `CB 1B` | 8 |  |
| `RR H` | `CB 1C` | 8 |  |
| `RR L` | `CB 1D` | 8 |  |
| `RR A` | `CB 1F` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
