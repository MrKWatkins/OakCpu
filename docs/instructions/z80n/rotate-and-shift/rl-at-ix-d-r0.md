# RL (IX + d), r

Rotates the byte at (IX + d) left one bit through the carry flag, copying the carry flag to bit 0 and bit 7 to the carry flag, and stores the result in both the byte at (IX + d) and r.

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
| `RL (IX + d), B` | `DD CB d 10` | 23 |  |
| `RL (IX + d), C` | `DD CB d 11` | 23 |  |
| `RL (IX + d), D` | `DD CB d 12` | 23 |  |
| `RL (IX + d), E` | `DD CB d 13` | 23 |  |
| `RL (IX + d), H` | `DD CB d 14` | 23 |  |
| `RL (IX + d), L` | `DD CB d 15` | 23 |  |
| `RL (IX + d), A` | `DD CB d 17` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
