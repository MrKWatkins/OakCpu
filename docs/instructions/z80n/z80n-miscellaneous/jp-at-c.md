# JP (C)

Jumps to the address formed from the top two bits of PC and the byte read from the port addressed by BC shifted left six bits.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Unaffected. |
| H | Unaffected. |
| X | Unaffected. |
| PV | Unaffected. |
| N | Unaffected. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `JP (C)` | `ED 98` | 13 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
