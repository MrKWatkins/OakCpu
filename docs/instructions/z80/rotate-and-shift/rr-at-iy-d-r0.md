# RR (IY + d), r

Rotates the byte at (IY + d) right one bit through the carry flag, copying the carry flag to bit 7 and bit 0 to the carry flag, and stores the result in both the byte at (IY + d) and r.

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
| `RR (IY + d), B` | `FD CB d 18` | 23 |  |
| `RR (IY + d), C` | `FD CB d 19` | 23 |  |
| `RR (IY + d), D` | `FD CB d 1A` | 23 |  |
| `RR (IY + d), E` | `FD CB d 1B` | 23 |  |
| `RR (IY + d), H` | `FD CB d 1C` | 23 |  |
| `RR (IY + d), L` | `FD CB d 1D` | 23 |  |
| `RR (IY + d), A` | `FD CB d 1F` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
