# RES b, (IX + d)

Resets bit b of the byte at (IX + d).

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
| `RES 0, (IX + d)` | `DD CB d 86` | 23 |  |
| `RES 1, (IX + d)` | `DD CB d 8E` | 23 |  |
| `RES 2, (IX + d)` | `DD CB d 96` | 23 |  |
| `RES 3, (IX + d)` | `DD CB d 9E` | 23 |  |
| `RES 4, (IX + d)` | `DD CB d A6` | 23 |  |
| `RES 5, (IX + d)` | `DD CB d AE` | 23 |  |
| `RES 6, (IX + d)` | `DD CB d B6` | 23 |  |
| `RES 7, (IX + d)` | `DD CB d BE` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
