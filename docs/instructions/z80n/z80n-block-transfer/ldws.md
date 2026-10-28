# LDWS

Copies the byte at (HL) to (DE), then increments L and D, stepping down a column of ZX Spectrum screen memory.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of D is set, otherwise reset. |
| Z | Set if D is zero, otherwise reset. |
| Y | Copied from bit 5 of D. |
| H | Set if (D & 0x0F) equals 0x00, otherwise reset. |
| X | Copied from bit 3 of D. |
| PV | Set if D equals 0x80, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `LDWS` | `ED A5` | 14 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
