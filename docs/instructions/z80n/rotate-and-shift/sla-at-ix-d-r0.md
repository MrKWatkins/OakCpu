# SLA (IX + d), r

Shifts the byte at (IX + d) left one bit, copying bit 7 to the carry flag and resetting bit 0, and stores the result in both the byte at (IX + d) and r.

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
| `SLA (IX + d), B` | `DD CB d 20` | 23 |  |
| `SLA (IX + d), C` | `DD CB d 21` | 23 |  |
| `SLA (IX + d), D` | `DD CB d 22` | 23 |  |
| `SLA (IX + d), E` | `DD CB d 23` | 23 |  |
| `SLA (IX + d), H` | `DD CB d 24` | 23 |  |
| `SLA (IX + d), L` | `DD CB d 25` | 23 |  |
| `SLA (IX + d), A` | `DD CB d 27` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
