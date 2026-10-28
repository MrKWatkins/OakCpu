# LD r, r'

Copies r' into r.

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
| `LD B, B` | `40` | 4 |  |
| `LD B, C` | `41` | 4 |  |
| `LD B, D` | `42` | 4 |  |
| `LD B, E` | `43` | 4 |  |
| `LD B, H` | `44` | 4 |  |
| `LD B, L` | `45` | 4 |  |
| `LD B, A` | `47` | 4 |  |
| `LD C, B` | `48` | 4 |  |
| `LD C, C` | `49` | 4 |  |
| `LD C, D` | `4A` | 4 |  |
| `LD C, E` | `4B` | 4 |  |
| `LD C, H` | `4C` | 4 |  |
| `LD C, L` | `4D` | 4 |  |
| `LD C, A` | `4F` | 4 |  |
| `LD D, B` | `50` | 4 |  |
| `LD D, C` | `51` | 4 |  |
| `LD D, D` | `52` | 4 |  |
| `LD D, E` | `53` | 4 |  |
| `LD D, H` | `54` | 4 |  |
| `LD D, L` | `55` | 4 |  |
| `LD D, A` | `57` | 4 |  |
| `LD E, B` | `58` | 4 |  |
| `LD E, C` | `59` | 4 |  |
| `LD E, D` | `5A` | 4 |  |
| `LD E, E` | `5B` | 4 |  |
| `LD E, H` | `5C` | 4 |  |
| `LD E, L` | `5D` | 4 |  |
| `LD E, A` | `5F` | 4 |  |
| `LD H, B` | `60` | 4 |  |
| `LD H, C` | `61` | 4 |  |
| `LD H, D` | `62` | 4 |  |
| `LD H, E` | `63` | 4 |  |
| `LD H, H` | `64` | 4 |  |
| `LD H, L` | `65` | 4 |  |
| `LD H, A` | `67` | 4 |  |
| `LD L, B` | `68` | 4 |  |
| `LD L, C` | `69` | 4 |  |
| `LD L, D` | `6A` | 4 |  |
| `LD L, E` | `6B` | 4 |  |
| `LD L, H` | `6C` | 4 |  |
| `LD L, L` | `6D` | 4 |  |
| `LD L, A` | `6F` | 4 |  |
| `LD A, B` | `78` | 4 |  |
| `LD A, C` | `79` | 4 |  |
| `LD A, D` | `7A` | 4 |  |
| `LD A, E` | `7B` | 4 |  |
| `LD A, H` | `7C` | 4 |  |
| `LD A, L` | `7D` | 4 |  |
| `LD A, A` | `7F` | 4 |  |
| `LD B, B` | `DD 40` | 8 | Undocumented. |
| `LD B, C` | `DD 41` | 8 | Undocumented. |
| `LD B, D` | `DD 42` | 8 | Undocumented. |
| `LD B, E` | `DD 43` | 8 | Undocumented. |
| `LD B, IXH` | `DD 44` | 8 | Undocumented. |
| `LD B, IXL` | `DD 45` | 8 | Undocumented. |
| `LD B, A` | `DD 47` | 8 | Undocumented. |
| `LD C, B` | `DD 48` | 8 | Undocumented. |
| `LD C, C` | `DD 49` | 8 | Undocumented. |
| `LD C, D` | `DD 4A` | 8 | Undocumented. |
| `LD C, E` | `DD 4B` | 8 | Undocumented. |
| `LD C, IXH` | `DD 4C` | 8 | Undocumented. |
| `LD C, IXL` | `DD 4D` | 8 | Undocumented. |
| `LD C, A` | `DD 4F` | 8 | Undocumented. |
| `LD D, B` | `DD 50` | 8 | Undocumented. |
| `LD D, C` | `DD 51` | 8 | Undocumented. |
| `LD D, D` | `DD 52` | 8 | Undocumented. |
| `LD D, E` | `DD 53` | 8 | Undocumented. |
| `LD D, IXH` | `DD 54` | 8 | Undocumented. |
| `LD D, IXL` | `DD 55` | 8 | Undocumented. |
| `LD D, A` | `DD 57` | 8 | Undocumented. |
| `LD E, B` | `DD 58` | 8 | Undocumented. |
| `LD E, C` | `DD 59` | 8 | Undocumented. |
| `LD E, D` | `DD 5A` | 8 | Undocumented. |
| `LD E, E` | `DD 5B` | 8 | Undocumented. |
| `LD E, IXH` | `DD 5C` | 8 | Undocumented. |
| `LD E, IXL` | `DD 5D` | 8 | Undocumented. |
| `LD E, A` | `DD 5F` | 8 | Undocumented. |
| `LD IXH, B` | `DD 60` | 8 | Undocumented. |
| `LD IXH, C` | `DD 61` | 8 | Undocumented. |
| `LD IXH, D` | `DD 62` | 8 | Undocumented. |
| `LD IXH, E` | `DD 63` | 8 | Undocumented. |
| `LD IXH, IXH` | `DD 64` | 8 | Undocumented. |
| `LD IXH, IXL` | `DD 65` | 8 | Undocumented. |
| `LD IXH, A` | `DD 67` | 8 | Undocumented. |
| `LD IXL, B` | `DD 68` | 8 | Undocumented. |
| `LD IXL, C` | `DD 69` | 8 | Undocumented. |
| `LD IXL, D` | `DD 6A` | 8 | Undocumented. |
| `LD IXL, E` | `DD 6B` | 8 | Undocumented. |
| `LD IXL, IXH` | `DD 6C` | 8 | Undocumented. |
| `LD IXL, IXL` | `DD 6D` | 8 | Undocumented. |
| `LD IXL, A` | `DD 6F` | 8 | Undocumented. |
| `LD A, B` | `DD 78` | 8 | Undocumented. |
| `LD A, C` | `DD 79` | 8 | Undocumented. |
| `LD A, D` | `DD 7A` | 8 | Undocumented. |
| `LD A, E` | `DD 7B` | 8 | Undocumented. |
| `LD A, IXH` | `DD 7C` | 8 | Undocumented. |
| `LD A, IXL` | `DD 7D` | 8 | Undocumented. |
| `LD A, A` | `DD 7F` | 8 | Undocumented. |
| `LD B, B` | `FD 40` | 8 | Undocumented. |
| `LD B, C` | `FD 41` | 8 | Undocumented. |
| `LD B, D` | `FD 42` | 8 | Undocumented. |
| `LD B, E` | `FD 43` | 8 | Undocumented. |
| `LD B, IYH` | `FD 44` | 8 | Undocumented. |
| `LD B, IYL` | `FD 45` | 8 | Undocumented. |
| `LD B, A` | `FD 47` | 8 | Undocumented. |
| `LD C, B` | `FD 48` | 8 | Undocumented. |
| `LD C, C` | `FD 49` | 8 | Undocumented. |
| `LD C, D` | `FD 4A` | 8 | Undocumented. |
| `LD C, E` | `FD 4B` | 8 | Undocumented. |
| `LD C, IYH` | `FD 4C` | 8 | Undocumented. |
| `LD C, IYL` | `FD 4D` | 8 | Undocumented. |
| `LD C, A` | `FD 4F` | 8 | Undocumented. |
| `LD D, B` | `FD 50` | 8 | Undocumented. |
| `LD D, C` | `FD 51` | 8 | Undocumented. |
| `LD D, D` | `FD 52` | 8 | Undocumented. |
| `LD D, E` | `FD 53` | 8 | Undocumented. |
| `LD D, IYH` | `FD 54` | 8 | Undocumented. |
| `LD D, IYL` | `FD 55` | 8 | Undocumented. |
| `LD D, A` | `FD 57` | 8 | Undocumented. |
| `LD E, B` | `FD 58` | 8 | Undocumented. |
| `LD E, C` | `FD 59` | 8 | Undocumented. |
| `LD E, D` | `FD 5A` | 8 | Undocumented. |
| `LD E, E` | `FD 5B` | 8 | Undocumented. |
| `LD E, IYH` | `FD 5C` | 8 | Undocumented. |
| `LD E, IYL` | `FD 5D` | 8 | Undocumented. |
| `LD E, A` | `FD 5F` | 8 | Undocumented. |
| `LD IYH, B` | `FD 60` | 8 | Undocumented. |
| `LD IYH, C` | `FD 61` | 8 | Undocumented. |
| `LD IYH, D` | `FD 62` | 8 | Undocumented. |
| `LD IYH, E` | `FD 63` | 8 | Undocumented. |
| `LD IYH, IYH` | `FD 64` | 8 | Undocumented. |
| `LD IYH, IYL` | `FD 65` | 8 | Undocumented. |
| `LD IYH, A` | `FD 67` | 8 | Undocumented. |
| `LD IYL, B` | `FD 68` | 8 | Undocumented. |
| `LD IYL, C` | `FD 69` | 8 | Undocumented. |
| `LD IYL, D` | `FD 6A` | 8 | Undocumented. |
| `LD IYL, E` | `FD 6B` | 8 | Undocumented. |
| `LD IYL, IYH` | `FD 6C` | 8 | Undocumented. |
| `LD IYL, IYL` | `FD 6D` | 8 | Undocumented. |
| `LD IYL, A` | `FD 6F` | 8 | Undocumented. |
| `LD A, B` | `FD 78` | 8 | Undocumented. |
| `LD A, C` | `FD 79` | 8 | Undocumented. |
| `LD A, D` | `FD 7A` | 8 | Undocumented. |
| `LD A, E` | `FD 7B` | 8 | Undocumented. |
| `LD A, IYH` | `FD 7C` | 8 | Undocumented. |
| `LD A, IYL` | `FD 7D` | 8 | Undocumented. |
| `LD A, A` | `FD 7F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
