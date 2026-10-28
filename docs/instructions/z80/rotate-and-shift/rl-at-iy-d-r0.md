# RL (IY + d), r

Rotates the byte at (IY + d) left one bit through the carry flag, copying the carry flag to bit 0 and bit 7 to the carry flag, and stores the result in both the byte at (IY + d) and r.

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
| C | Set if bit 7 of the value read is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `RL (IY + d), B` | `FD CB d 10` | 23 |  |
| `RL (IY + d), C` | `FD CB d 11` | 23 |  |
| `RL (IY + d), D` | `FD CB d 12` | 23 |  |
| `RL (IY + d), E` | `FD CB d 13` | 23 |  |
| `RL (IY + d), H` | `FD CB d 14` | 23 |  |
| `RL (IY + d), L` | `FD CB d 15` | 23 |  |
| `RL (IY + d), A` | `FD CB d 17` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
