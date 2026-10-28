# IN (C)

Reads a byte from the port addressed by BC, setting the flags from it and discarding the value.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the value read. |
| Z | Set if the value read is zero, otherwise reset. |
| Y | Copied from bit 5 of the value read. |
| H | Reset. |
| X | Copied from bit 3 of the value read. |
| PV | Set if the value read has even parity, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `IN (C)` | `ED 70` | 12 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
