# SBC HL, rr

Subtracts rr and the carry flag from HL.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the high byte of the result. |
| Z | Set if the 16-bit result is zero, otherwise reset. |
| Y | Copied from bit 5 of the high byte of the result. |
| H | Set if there was a borrow from bit 12, otherwise reset. |
| X | Copied from bit 3 of the high byte of the result. |
| PV | Set if the subtraction overflowed, otherwise reset. |
| N | Set. |
| C | Set if bit 16 of the result is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `SBC HL, BC` | `ED 42` | 15 |  |
| `SBC HL, DE` | `ED 52` | 15 |  |
| `SBC HL, HL` | `ED 62` | 15 |  |
| `SBC HL, SP` | `ED 72` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
