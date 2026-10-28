# PUSH $nn

Pushes nn onto the stack. Unlike other instructions the high byte of nn comes first in the encoding.

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
| `PUSH $nn` | `ED 8A nn` | 23 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
