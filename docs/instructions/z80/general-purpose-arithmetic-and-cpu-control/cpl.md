# CPL

Inverts every bit of A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Copied from bit 5 of A. |
| H | Set. |
| X | Copied from bit 3 of A. |
| PV | Unaffected. |
| N | Set. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `CPL` | `2F` | 4 |  |
| `CPL` | `DD 2F` | 8 | Undocumented. |
| `CPL` | `FD 2F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
