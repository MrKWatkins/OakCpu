# IN r, (C)

Reads a byte from the port addressed by BC into r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the value read. |
| Z | Set if the value read is zero, otherwise reset. |
| Y | Copied from bit 5 of the value read. |
| H | Reset. |
| X | Copied from bit 3 of the value read. |
| PV | Set if the value read has even parity, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `IN B, (C)` | `ED 40` | 12 |  |
| `IN C, (C)` | `ED 48` | 12 |  |
| `IN D, (C)` | `ED 50` | 12 |  |
| `IN E, (C)` | `ED 58` | 12 |  |
| `IN H, (C)` | `ED 60` | 12 |  |
| `IN L, (C)` | `ED 68` | 12 |  |
| `IN A, (C)` | `ED 78` | 12 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
