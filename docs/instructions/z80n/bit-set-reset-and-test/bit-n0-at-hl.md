# BIT b, (HL)

Tests bit b of the byte at (HL), setting the Z flag if it is zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the tested bit. |
| Z | Set if the tested bit is zero, otherwise reset. |
| Y | Copied from bit 5 of W. |
| H | Set. |
| X | Copied from bit 3 of W. |
| PV | Set if the tested bit is zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `BIT 0, (HL)` | `CB 46` | 12 |  |
| `BIT 1, (HL)` | `CB 4E` | 12 |  |
| `BIT 2, (HL)` | `CB 56` | 12 |  |
| `BIT 3, (HL)` | `CB 5E` | 12 |  |
| `BIT 4, (HL)` | `CB 66` | 12 |  |
| `BIT 5, (HL)` | `CB 6E` | 12 |  |
| `BIT 6, (HL)` | `CB 76` | 12 |  |
| `BIT 7, (HL)` | `CB 7E` | 12 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
