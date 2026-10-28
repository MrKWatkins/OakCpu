# DEC r

Decrements r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of r is set, otherwise reset. |
| Z | Set if r is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Set if (r & 0x0F) equals 0x0F, otherwise reset. |
| X | Copied from bit 3 of r. |
| PV | Set if r equals 0x7F, otherwise reset. |
| N | Set. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `DEC B` | `05` | 4 |  |
| `DEC C` | `0D` | 4 |  |
| `DEC D` | `15` | 4 |  |
| `DEC E` | `1D` | 4 |  |
| `DEC H` | `25` | 4 |  |
| `DEC L` | `2D` | 4 |  |
| `DEC A` | `3D` | 4 |  |
| `DEC B` | `DD 05` | 8 | Undocumented. |
| `DEC C` | `DD 0D` | 8 | Undocumented. |
| `DEC D` | `DD 15` | 8 | Undocumented. |
| `DEC E` | `DD 1D` | 8 | Undocumented. |
| `DEC IXH` | `DD 25` | 8 | Undocumented. |
| `DEC IXL` | `DD 2D` | 8 | Undocumented. |
| `DEC A` | `DD 3D` | 8 | Undocumented. |
| `DEC B` | `FD 05` | 8 | Undocumented. |
| `DEC C` | `FD 0D` | 8 | Undocumented. |
| `DEC D` | `FD 15` | 8 | Undocumented. |
| `DEC E` | `FD 1D` | 8 | Undocumented. |
| `DEC IYH` | `FD 25` | 8 | Undocumented. |
| `DEC IYL` | `FD 2D` | 8 | Undocumented. |
| `DEC A` | `FD 3D` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
