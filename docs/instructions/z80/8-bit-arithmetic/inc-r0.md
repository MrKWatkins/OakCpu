# INC r

Increments r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of r is set, otherwise reset. |
| Z | Set if r is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Set if (r & 0x0F) equals 0x00, otherwise reset. |
| X | Copied from bit 3 of r. |
| PV | Set if r equals 0x80, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `INC B` | `04` | 4 |  |
| `INC C` | `0C` | 4 |  |
| `INC D` | `14` | 4 |  |
| `INC E` | `1C` | 4 |  |
| `INC H` | `24` | 4 |  |
| `INC L` | `2C` | 4 |  |
| `INC A` | `3C` | 4 |  |
| `INC B` | `DD 04` | 8 | Undocumented. |
| `INC C` | `DD 0C` | 8 | Undocumented. |
| `INC D` | `DD 14` | 8 | Undocumented. |
| `INC E` | `DD 1C` | 8 | Undocumented. |
| `INC IXH` | `DD 24` | 8 | Undocumented. |
| `INC IXL` | `DD 2C` | 8 | Undocumented. |
| `INC A` | `DD 3C` | 8 | Undocumented. |
| `INC B` | `FD 04` | 8 | Undocumented. |
| `INC C` | `FD 0C` | 8 | Undocumented. |
| `INC D` | `FD 14` | 8 | Undocumented. |
| `INC E` | `FD 1C` | 8 | Undocumented. |
| `INC IYH` | `FD 24` | 8 | Undocumented. |
| `INC IYL` | `FD 2C` | 8 | Undocumented. |
| `INC A` | `FD 3C` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
