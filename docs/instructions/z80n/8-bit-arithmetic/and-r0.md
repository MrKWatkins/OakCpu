# AND r

Sets A to the bitwise AND of A and r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of A. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of A. |
| H | Set. |
| X | Copied from bit 3 of A. |
| PV | Set if A has even parity, otherwise reset. |
| N | Reset. |
| C | Reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `AND B` | `A0` | 4 |  |
| `AND C` | `A1` | 4 |  |
| `AND D` | `A2` | 4 |  |
| `AND E` | `A3` | 4 |  |
| `AND H` | `A4` | 4 |  |
| `AND L` | `A5` | 4 |  |
| `AND A` | `A7` | 4 |  |
| `AND B` | `DD A0` | 8 | Undocumented. |
| `AND C` | `DD A1` | 8 | Undocumented. |
| `AND D` | `DD A2` | 8 | Undocumented. |
| `AND E` | `DD A3` | 8 | Undocumented. |
| `AND IXH` | `DD A4` | 8 | Undocumented. |
| `AND IXL` | `DD A5` | 8 | Undocumented. |
| `AND A` | `DD A7` | 8 | Undocumented. |
| `AND B` | `FD A0` | 8 | Undocumented. |
| `AND C` | `FD A1` | 8 | Undocumented. |
| `AND D` | `FD A2` | 8 | Undocumented. |
| `AND E` | `FD A3` | 8 | Undocumented. |
| `AND IYH` | `FD A4` | 8 | Undocumented. |
| `AND IYL` | `FD A5` | 8 | Undocumented. |
| `AND A` | `FD A7` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
