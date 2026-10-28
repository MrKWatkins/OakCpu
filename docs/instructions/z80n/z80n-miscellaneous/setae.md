# SETAE

Sets A to a mask with the single bit selected by the low three bits of E set, with zero selecting bit 7.

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
| `SETAE` | `ED 95` | 8 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
