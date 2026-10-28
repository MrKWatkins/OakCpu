# RLC (IX + d), r

Rotates the byte at (IX + d) left one bit, copying bit 7 to both bit 0 and the carry flag, and stores the result in both the byte at (IX + d) and r.

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
| `RLC (IX + d), B` | `DD CB d 00` | 23 |  |
| `RLC (IX + d), C` | `DD CB d 01` | 23 |  |
| `RLC (IX + d), D` | `DD CB d 02` | 23 |  |
| `RLC (IX + d), E` | `DD CB d 03` | 23 |  |
| `RLC (IX + d), H` | `DD CB d 04` | 23 |  |
| `RLC (IX + d), L` | `DD CB d 05` | 23 |  |
| `RLC (IX + d), A` | `DD CB d 07` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
