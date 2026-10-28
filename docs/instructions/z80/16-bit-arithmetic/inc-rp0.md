# INC rr

Increments rr.

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
| `INC BC` | `03` | 6 |  |
| `INC DE` | `13` | 6 |  |
| `INC HL` | `23` | 6 |  |
| `INC SP` | `33` | 6 |  |
| `INC BC` | `DD 03` | 10 | Undocumented. |
| `INC DE` | `DD 13` | 10 | Undocumented. |
| `INC IX` | `DD 23` | 10 |  |
| `INC SP` | `DD 33` | 10 | Undocumented. |
| `INC BC` | `FD 03` | 10 | Undocumented. |
| `INC DE` | `FD 13` | 10 | Undocumented. |
| `INC IY` | `FD 23` | 10 |  |
| `INC SP` | `FD 33` | 10 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
