# BIT b, (IY + d)

Tests bit b of the byte at (IY + d), setting the Z flag if it is zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the tested bit. |
| Z | Set if the tested bit is zero, otherwise reset. |
| Y | Copied from bit 5 of W. |
| H | Set. |
| X | Copied from bit 3 of W. |
| PV | Set if the tested bit is zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `BIT 0, (IY + d)` | `FD CB d 40` | 20 | Undocumented. |
| `BIT 0, (IY + d)` | `FD CB d 41` | 20 | Undocumented. |
| `BIT 0, (IY + d)` | `FD CB d 42` | 20 | Undocumented. |
| `BIT 0, (IY + d)` | `FD CB d 43` | 20 | Undocumented. |
| `BIT 0, (IY + d)` | `FD CB d 44` | 20 | Undocumented. |
| `BIT 0, (IY + d)` | `FD CB d 45` | 20 | Undocumented. |
| `BIT 0, (IY + d)` | `FD CB d 46` | 20 |  |
| `BIT 0, (IY + d)` | `FD CB d 47` | 20 | Undocumented. |
| `BIT 1, (IY + d)` | `FD CB d 48` | 20 | Undocumented. |
| `BIT 1, (IY + d)` | `FD CB d 49` | 20 | Undocumented. |
| `BIT 1, (IY + d)` | `FD CB d 4A` | 20 | Undocumented. |
| `BIT 1, (IY + d)` | `FD CB d 4B` | 20 | Undocumented. |
| `BIT 1, (IY + d)` | `FD CB d 4C` | 20 | Undocumented. |
| `BIT 1, (IY + d)` | `FD CB d 4D` | 20 | Undocumented. |
| `BIT 1, (IY + d)` | `FD CB d 4E` | 20 |  |
| `BIT 1, (IY + d)` | `FD CB d 4F` | 20 | Undocumented. |
| `BIT 2, (IY + d)` | `FD CB d 50` | 20 | Undocumented. |
| `BIT 2, (IY + d)` | `FD CB d 51` | 20 | Undocumented. |
| `BIT 2, (IY + d)` | `FD CB d 52` | 20 | Undocumented. |
| `BIT 2, (IY + d)` | `FD CB d 53` | 20 | Undocumented. |
| `BIT 2, (IY + d)` | `FD CB d 54` | 20 | Undocumented. |
| `BIT 2, (IY + d)` | `FD CB d 55` | 20 | Undocumented. |
| `BIT 2, (IY + d)` | `FD CB d 56` | 20 |  |
| `BIT 2, (IY + d)` | `FD CB d 57` | 20 | Undocumented. |
| `BIT 3, (IY + d)` | `FD CB d 58` | 20 | Undocumented. |
| `BIT 3, (IY + d)` | `FD CB d 59` | 20 | Undocumented. |
| `BIT 3, (IY + d)` | `FD CB d 5A` | 20 | Undocumented. |
| `BIT 3, (IY + d)` | `FD CB d 5B` | 20 | Undocumented. |
| `BIT 3, (IY + d)` | `FD CB d 5C` | 20 | Undocumented. |
| `BIT 3, (IY + d)` | `FD CB d 5D` | 20 | Undocumented. |
| `BIT 3, (IY + d)` | `FD CB d 5E` | 20 |  |
| `BIT 3, (IY + d)` | `FD CB d 5F` | 20 | Undocumented. |
| `BIT 4, (IY + d)` | `FD CB d 60` | 20 | Undocumented. |
| `BIT 4, (IY + d)` | `FD CB d 61` | 20 | Undocumented. |
| `BIT 4, (IY + d)` | `FD CB d 62` | 20 | Undocumented. |
| `BIT 4, (IY + d)` | `FD CB d 63` | 20 | Undocumented. |
| `BIT 4, (IY + d)` | `FD CB d 64` | 20 | Undocumented. |
| `BIT 4, (IY + d)` | `FD CB d 65` | 20 | Undocumented. |
| `BIT 4, (IY + d)` | `FD CB d 66` | 20 |  |
| `BIT 4, (IY + d)` | `FD CB d 67` | 20 | Undocumented. |
| `BIT 5, (IY + d)` | `FD CB d 68` | 20 | Undocumented. |
| `BIT 5, (IY + d)` | `FD CB d 69` | 20 | Undocumented. |
| `BIT 5, (IY + d)` | `FD CB d 6A` | 20 | Undocumented. |
| `BIT 5, (IY + d)` | `FD CB d 6B` | 20 | Undocumented. |
| `BIT 5, (IY + d)` | `FD CB d 6C` | 20 | Undocumented. |
| `BIT 5, (IY + d)` | `FD CB d 6D` | 20 | Undocumented. |
| `BIT 5, (IY + d)` | `FD CB d 6E` | 20 |  |
| `BIT 5, (IY + d)` | `FD CB d 6F` | 20 | Undocumented. |
| `BIT 6, (IY + d)` | `FD CB d 70` | 20 | Undocumented. |
| `BIT 6, (IY + d)` | `FD CB d 71` | 20 | Undocumented. |
| `BIT 6, (IY + d)` | `FD CB d 72` | 20 | Undocumented. |
| `BIT 6, (IY + d)` | `FD CB d 73` | 20 | Undocumented. |
| `BIT 6, (IY + d)` | `FD CB d 74` | 20 | Undocumented. |
| `BIT 6, (IY + d)` | `FD CB d 75` | 20 | Undocumented. |
| `BIT 6, (IY + d)` | `FD CB d 76` | 20 |  |
| `BIT 6, (IY + d)` | `FD CB d 77` | 20 | Undocumented. |
| `BIT 7, (IY + d)` | `FD CB d 78` | 20 | Undocumented. |
| `BIT 7, (IY + d)` | `FD CB d 79` | 20 | Undocumented. |
| `BIT 7, (IY + d)` | `FD CB d 7A` | 20 | Undocumented. |
| `BIT 7, (IY + d)` | `FD CB d 7B` | 20 | Undocumented. |
| `BIT 7, (IY + d)` | `FD CB d 7C` | 20 | Undocumented. |
| `BIT 7, (IY + d)` | `FD CB d 7D` | 20 | Undocumented. |
| `BIT 7, (IY + d)` | `FD CB d 7E` | 20 |  |
| `BIT 7, (IY + d)` | `FD CB d 7F` | 20 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
