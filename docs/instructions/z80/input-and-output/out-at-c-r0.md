# OUT (C), r

Writes r to the port addressed by BC.

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
| `OUT (C), B` | `ED 41` | 12 |  |
| `OUT (C), C` | `ED 49` | 12 |  |
| `OUT (C), D` | `ED 51` | 12 |  |
| `OUT (C), E` | `ED 59` | 12 |  |
| `OUT (C), H` | `ED 61` | 12 |  |
| `OUT (C), L` | `ED 69` | 12 |  |
| `OUT (C), A` | `ED 79` | 12 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
