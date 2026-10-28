# RES b, (HL)

Resets bit b of the byte at (HL).

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Unaffected. |
| H | Unaffected. |
| X | Unaffected. |
| PV | Unaffected. |
| N | Unaffected. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `RES 0, (HL)` | `CB 86` | 15 |  |
| `RES 1, (HL)` | `CB 8E` | 15 |  |
| `RES 2, (HL)` | `CB 96` | 15 |  |
| `RES 3, (HL)` | `CB 9E` | 15 |  |
| `RES 4, (HL)` | `CB A6` | 15 |  |
| `RES 5, (HL)` | `CB AE` | 15 |  |
| `RES 6, (HL)` | `CB B6` | 15 |  |
| `RES 7, (HL)` | `CB BE` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
