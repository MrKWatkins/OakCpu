# LDPIRX

Copies the byte at ((HL & 0xFFF8) + (E & 7)), i.e. from the eight byte aligned pattern at HL, to (DE) unless it equals A, then increments DE and decrements BC, repeating until BC is zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | When the instruction completes, set if bit 1 of the result is set, otherwise reset. When it repeats, set if bit 5 of PCH is set, otherwise reset. |
| H | Reset. |
| X | When the instruction completes, set if bit 3 of the result is set, otherwise reset. When it repeats, set if bit 3 of PCH is set, otherwise reset. |
| PV | Set if BC is not zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `LDPIRX` | `ED B7` | 16 or 21 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
