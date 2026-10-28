# LDD

Copies the byte at (HL) to (DE), then decrements HL, DE and BC.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Set if bit 1 of the result is set, otherwise reset. |
| H | Reset. |
| X | Set if bit 3 of the result is set, otherwise reset. |
| PV | Set if BC is not zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `LDD` | `ED A8` | 16 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
