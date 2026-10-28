# RRC (IY + d)

Rotates the byte at (IY + d) right one bit; bit 0 is copied to both bit 7 and the carry flag.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the result has even parity, otherwise reset. |
| N | Reset. |
| C | Copied from bit 0 of the value read. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `RRC (IY + d)` | `FD CB d 0E` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
