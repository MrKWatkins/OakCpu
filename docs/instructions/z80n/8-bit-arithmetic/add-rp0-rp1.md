# ADD rr, rr'

Adds rr' to rr.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Copied from bit 5 of the high byte of the result. |
| H | Set if there was a carry from bit 11 to bit 12, otherwise reset. |
| X | Copied from bit 3 of the high byte of the result. |
| PV | Unaffected. |
| N | Reset. |
| C | Set if bit 16 of the result is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `ADD HL, BC` | `09` | 11 |  |
| `ADD HL, DE` | `19` | 11 |  |
| `ADD HL, HL` | `29` | 11 |  |
| `ADD HL, SP` | `39` | 11 |  |
| `ADD IX, BC` | `DD 09` | 15 |  |
| `ADD IX, DE` | `DD 19` | 15 |  |
| `ADD IX, IX` | `DD 29` | 15 |  |
| `ADD IX, SP` | `DD 39` | 15 |  |
| `ADD IY, BC` | `FD 09` | 15 |  |
| `ADD IY, DE` | `FD 19` | 15 |  |
| `ADD IY, IY` | `FD 29` | 15 |  |
| `ADD IY, SP` | `FD 39` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
