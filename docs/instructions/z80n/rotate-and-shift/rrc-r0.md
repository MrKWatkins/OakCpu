# RRC r

Rotates r right one bit; bit 0 is copied to both bit 7 and the carry flag.

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
| `RRC B` | `CB 08` | 8 |  |
| `RRC C` | `CB 09` | 8 |  |
| `RRC D` | `CB 0A` | 8 |  |
| `RRC E` | `CB 0B` | 8 |  |
| `RRC H` | `CB 0C` | 8 |  |
| `RRC L` | `CB 0D` | 8 |  |
| `RRC A` | `CB 0F` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
