# LDDX

Copies the byte at (HL) to (DE) unless it equals A, then decrements HL, increments DE and decrements BC.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Set if bit 1 of the result is set, otherwise reset. |
| H | Reset. |
| X | Set if bit 3 of the result is set, otherwise reset. |
| PV | Set if BC is not zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `LDDX` | `ED AC` | 16 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
