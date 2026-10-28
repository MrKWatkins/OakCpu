# SRA (IY + d), r

Shifts the byte at (IY + d) right one bit, copying bit 0 to the carry flag and keeping bit 7 to preserve the sign, and stores the result in both the byte at (IY + d) and r.

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
| `SRA (IY + d), B` | `FD CB d 28` | 23 |  |
| `SRA (IY + d), C` | `FD CB d 29` | 23 |  |
| `SRA (IY + d), D` | `FD CB d 2A` | 23 |  |
| `SRA (IY + d), E` | `FD CB d 2B` | 23 |  |
| `SRA (IY + d), H` | `FD CB d 2C` | 23 |  |
| `SRA (IY + d), L` | `FD CB d 2D` | 23 |  |
| `SRA (IY + d), A` | `FD CB d 2F` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
