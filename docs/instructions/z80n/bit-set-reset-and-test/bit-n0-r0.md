# BIT b, r

Tests bit b of r, setting the Z flag if it is zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the tested bit. |
| Z | Set if the tested bit is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Set. |
| X | Copied from bit 3 of r. |
| PV | Set if the tested bit is zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `BIT 0, B` | `CB 40` | 8 |  |
| `BIT 0, C` | `CB 41` | 8 |  |
| `BIT 0, D` | `CB 42` | 8 |  |
| `BIT 0, E` | `CB 43` | 8 |  |
| `BIT 0, H` | `CB 44` | 8 |  |
| `BIT 0, L` | `CB 45` | 8 |  |
| `BIT 0, A` | `CB 47` | 8 |  |
| `BIT 1, B` | `CB 48` | 8 |  |
| `BIT 1, C` | `CB 49` | 8 |  |
| `BIT 1, D` | `CB 4A` | 8 |  |
| `BIT 1, E` | `CB 4B` | 8 |  |
| `BIT 1, H` | `CB 4C` | 8 |  |
| `BIT 1, L` | `CB 4D` | 8 |  |
| `BIT 1, A` | `CB 4F` | 8 |  |
| `BIT 2, B` | `CB 50` | 8 |  |
| `BIT 2, C` | `CB 51` | 8 |  |
| `BIT 2, D` | `CB 52` | 8 |  |
| `BIT 2, E` | `CB 53` | 8 |  |
| `BIT 2, H` | `CB 54` | 8 |  |
| `BIT 2, L` | `CB 55` | 8 |  |
| `BIT 2, A` | `CB 57` | 8 |  |
| `BIT 3, B` | `CB 58` | 8 |  |
| `BIT 3, C` | `CB 59` | 8 |  |
| `BIT 3, D` | `CB 5A` | 8 |  |
| `BIT 3, E` | `CB 5B` | 8 |  |
| `BIT 3, H` | `CB 5C` | 8 |  |
| `BIT 3, L` | `CB 5D` | 8 |  |
| `BIT 3, A` | `CB 5F` | 8 |  |
| `BIT 4, B` | `CB 60` | 8 |  |
| `BIT 4, C` | `CB 61` | 8 |  |
| `BIT 4, D` | `CB 62` | 8 |  |
| `BIT 4, E` | `CB 63` | 8 |  |
| `BIT 4, H` | `CB 64` | 8 |  |
| `BIT 4, L` | `CB 65` | 8 |  |
| `BIT 4, A` | `CB 67` | 8 |  |
| `BIT 5, B` | `CB 68` | 8 |  |
| `BIT 5, C` | `CB 69` | 8 |  |
| `BIT 5, D` | `CB 6A` | 8 |  |
| `BIT 5, E` | `CB 6B` | 8 |  |
| `BIT 5, H` | `CB 6C` | 8 |  |
| `BIT 5, L` | `CB 6D` | 8 |  |
| `BIT 5, A` | `CB 6F` | 8 |  |
| `BIT 6, B` | `CB 70` | 8 |  |
| `BIT 6, C` | `CB 71` | 8 |  |
| `BIT 6, D` | `CB 72` | 8 |  |
| `BIT 6, E` | `CB 73` | 8 |  |
| `BIT 6, H` | `CB 74` | 8 |  |
| `BIT 6, L` | `CB 75` | 8 |  |
| `BIT 6, A` | `CB 77` | 8 |  |
| `BIT 7, B` | `CB 78` | 8 |  |
| `BIT 7, C` | `CB 79` | 8 |  |
| `BIT 7, D` | `CB 7A` | 8 |  |
| `BIT 7, E` | `CB 7B` | 8 |  |
| `BIT 7, H` | `CB 7C` | 8 |  |
| `BIT 7, L` | `CB 7D` | 8 |  |
| `BIT 7, A` | `CB 7F` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
