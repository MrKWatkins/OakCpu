# SET b, (IY + d)

Sets bit b of the byte at (IY + d).

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
| `SET 0, (IY + d)` | `FD CB d C6` | 23 |  |
| `SET 1, (IY + d)` | `FD CB d CE` | 23 |  |
| `SET 2, (IY + d)` | `FD CB d D6` | 23 |  |
| `SET 3, (IY + d)` | `FD CB d DE` | 23 |  |
| `SET 4, (IY + d)` | `FD CB d E6` | 23 |  |
| `SET 5, (IY + d)` | `FD CB d EE` | 23 |  |
| `SET 6, (IY + d)` | `FD CB d F6` | 23 |  |
| `SET 7, (IY + d)` | `FD CB d FE` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
