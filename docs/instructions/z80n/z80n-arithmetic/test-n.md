# TEST $n

Sets the flags from the bitwise AND of A and n, discarding the result.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set. |
| X | Copied from bit 3 of the result. |
| PV | Set if the result has even parity, otherwise reset. |
| N | Reset. |
| C | Reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `TEST $n` | `ED 27 n` | 11 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
