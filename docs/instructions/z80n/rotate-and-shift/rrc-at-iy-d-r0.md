# RRC (IY + d), r

Rotates the byte at (IY + d) right one bit, copying bit 0 to both bit 7 and the carry flag, and stores the result in both the byte at (IY + d) and r.

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
| `RRC (IY + d), B` | `FD CB d 08` | 23 |  |
| `RRC (IY + d), C` | `FD CB d 09` | 23 |  |
| `RRC (IY + d), D` | `FD CB d 0A` | 23 |  |
| `RRC (IY + d), E` | `FD CB d 0B` | 23 |  |
| `RRC (IY + d), H` | `FD CB d 0C` | 23 |  |
| `RRC (IY + d), L` | `FD CB d 0D` | 23 |  |
| `RRC (IY + d), A` | `FD CB d 0F` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
