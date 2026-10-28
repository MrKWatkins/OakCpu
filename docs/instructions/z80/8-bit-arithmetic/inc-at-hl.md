# INC (HL)

Increments the byte at (HL).

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of the result is set, otherwise reset. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set if (the result & 0x0F) equals 0x00, otherwise reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the result equals 0x80, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `INC (HL)` | `34` | 11 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
