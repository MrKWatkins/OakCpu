# SLA r

Shifts r left one bit; bit 7 is copied to the carry flag and bit 0 is reset.

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
| `SLA B` | `CB 20` | 8 |  |
| `SLA C` | `CB 21` | 8 |  |
| `SLA D` | `CB 22` | 8 |  |
| `SLA E` | `CB 23` | 8 |  |
| `SLA H` | `CB 24` | 8 |  |
| `SLA L` | `CB 25` | 8 |  |
| `SLA A` | `CB 27` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
