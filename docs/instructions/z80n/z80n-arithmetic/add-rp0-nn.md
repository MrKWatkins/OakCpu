# ADD rr, $nn

Adds nn to rr.

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
| `ADD HL, $nn` | `ED 34 nn` | 16 |  |
| `ADD DE, $nn` | `ED 35 nn` | 16 |  |
| `ADD BC, $nn` | `ED 36 nn` | 16 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
