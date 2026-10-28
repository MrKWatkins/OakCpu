# NEG

Negates A by subtracting it from zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of A. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of A. |
| H | Set if bit 4 of (the original value of A ^ A) is set, otherwise reset. |
| X | Copied from bit 3 of A. |
| PV | Set if A equals 0x80, otherwise reset. |
| N | Set. |
| C | Set if A is not zero, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `NEG` | `ED 44` | 8 |  |
| `NEG` | `ED 4C` | 8 | Undocumented. |
| `NEG` | `ED 54` | 8 | Undocumented. |
| `NEG` | `ED 5C` | 8 | Undocumented. |
| `NEG` | `ED 64` | 8 | Undocumented. |
| `NEG` | `ED 6C` | 8 | Undocumented. |
| `NEG` | `ED 74` | 8 | Undocumented. |
| `NEG` | `ED 7C` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
