# DEC (rr + d)

Decrements the byte at (rr + d).

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of the result is set, otherwise reset. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set if (the result & 0x0F) equals 0x0F, otherwise reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the result equals 0x7F, otherwise reset. |
| N | Set. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `DEC (IX + d)` | `DD 35 d` | 23 |  |
| `DEC (IY + d)` | `FD 35 d` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
