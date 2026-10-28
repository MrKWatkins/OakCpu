# OR r

Sets A to the bitwise OR of A and r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of A. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of A. |
| H | Reset. |
| X | Copied from bit 3 of A. |
| PV | Set if A has even parity, otherwise reset. |
| N | Reset. |
| C | Reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `OR B` | `B0` | 4 |  |
| `OR C` | `B1` | 4 |  |
| `OR D` | `B2` | 4 |  |
| `OR E` | `B3` | 4 |  |
| `OR H` | `B4` | 4 |  |
| `OR L` | `B5` | 4 |  |
| `OR A` | `B7` | 4 |  |
| `OR B` | `DD B0` | 8 | Undocumented. |
| `OR C` | `DD B1` | 8 | Undocumented. |
| `OR D` | `DD B2` | 8 | Undocumented. |
| `OR E` | `DD B3` | 8 | Undocumented. |
| `OR IXH` | `DD B4` | 8 | Undocumented. |
| `OR IXL` | `DD B5` | 8 | Undocumented. |
| `OR A` | `DD B7` | 8 | Undocumented. |
| `OR B` | `FD B0` | 8 | Undocumented. |
| `OR C` | `FD B1` | 8 | Undocumented. |
| `OR D` | `FD B2` | 8 | Undocumented. |
| `OR E` | `FD B3` | 8 | Undocumented. |
| `OR IYH` | `FD B4` | 8 | Undocumented. |
| `OR IYL` | `FD B5` | 8 | Undocumented. |
| `OR A` | `FD B7` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
