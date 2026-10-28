# LD A, r

Copies r into A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of r. |
| Z | Set if r is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Reset. |
| X | Copied from bit 3 of r. |
| PV | Set to the value of IFF2. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `LD A, I` | `ED 57` | 9 |  |
| `LD A, R` | `ED 5F` | 9 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
