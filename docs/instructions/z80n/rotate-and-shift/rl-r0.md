# RL r

Rotates r left one bit through the carry flag; the carry flag is copied to bit 0 and bit 7 to the carry flag.

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
| `RL B` | `CB 10` | 8 |  |
| `RL C` | `CB 11` | 8 |  |
| `RL D` | `CB 12` | 8 |  |
| `RL E` | `CB 13` | 8 |  |
| `RL H` | `CB 14` | 8 |  |
| `RL L` | `CB 15` | 8 |  |
| `RL A` | `CB 17` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
