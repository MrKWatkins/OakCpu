# SRL (IY + d), r

Shifts the byte at (IY + d) right one bit, copying bit 0 to the carry flag and resetting bit 7, and stores the result in both the byte at (IY + d) and r.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the result has even parity, otherwise reset. |
| N | Reset. |
| C | Copied from bit 0 of the value read. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `SRL (IY + d), B` | `FD CB d 38` | 23 |  |
| `SRL (IY + d), C` | `FD CB d 39` | 23 |  |
| `SRL (IY + d), D` | `FD CB d 3A` | 23 |  |
| `SRL (IY + d), E` | `FD CB d 3B` | 23 |  |
| `SRL (IY + d), H` | `FD CB d 3C` | 23 |  |
| `SRL (IY + d), L` | `FD CB d 3D` | 23 |  |
| `SRL (IY + d), A` | `FD CB d 3F` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
