# IM 0

Selects interrupt mode 0, in which the interrupting device supplies an instruction on the data bus for the CPU to execute.

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
| `IM 0` | `ED 46` | 8 |  |
| `IM 0` | `ED 4E` | 8 | Undocumented. |
| `IM 0` | `ED 66` | 8 | Undocumented. |
| `IM 0` | `ED 6E` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
