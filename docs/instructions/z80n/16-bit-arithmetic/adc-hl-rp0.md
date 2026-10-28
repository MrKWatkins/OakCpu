# ADC HL, rr

Adds rr and the carry flag to HL.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the high byte of the result. |
| Z | Set if the 16-bit result is zero, otherwise reset. |
| Y | Copied from bit 5 of the high byte of the result. |
| H | Set if there was a carry from bit 11 to bit 12, otherwise reset. |
| X | Copied from bit 3 of the high byte of the result. |
| PV | Set if the addition overflowed, otherwise reset. |
| N | Reset. |
| C | Set if bit 16 of the result is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `ADC HL, BC` | `ED 4A` | 15 |  |
| `ADC HL, DE` | `ED 5A` | 15 |  |
| `ADC HL, HL` | `ED 6A` | 15 |  |
| `ADC HL, SP` | `ED 7A` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
