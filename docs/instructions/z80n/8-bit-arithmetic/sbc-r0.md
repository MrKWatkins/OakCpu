# SBC r

Subtracts r and the carry flag from A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set if there was a borrow from bit 4, otherwise reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the subtraction overflowed, otherwise reset. |
| N | Set. |
| C | Set if bit 8 of the result is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `SBC B` | `98` | 4 |  |
| `SBC C` | `99` | 4 |  |
| `SBC D` | `9A` | 4 |  |
| `SBC E` | `9B` | 4 |  |
| `SBC H` | `9C` | 4 |  |
| `SBC L` | `9D` | 4 |  |
| `SBC A` | `9F` | 4 |  |
| `SBC B` | `DD 98` | 8 | Undocumented. |
| `SBC C` | `DD 99` | 8 | Undocumented. |
| `SBC D` | `DD 9A` | 8 | Undocumented. |
| `SBC E` | `DD 9B` | 8 | Undocumented. |
| `SBC IXH` | `DD 9C` | 8 | Undocumented. |
| `SBC IXL` | `DD 9D` | 8 | Undocumented. |
| `SBC A` | `DD 9F` | 8 | Undocumented. |
| `SBC B` | `FD 98` | 8 | Undocumented. |
| `SBC C` | `FD 99` | 8 | Undocumented. |
| `SBC D` | `FD 9A` | 8 | Undocumented. |
| `SBC E` | `FD 9B` | 8 | Undocumented. |
| `SBC IYH` | `FD 9C` | 8 | Undocumented. |
| `SBC IYL` | `FD 9D` | 8 | Undocumented. |
| `SBC A` | `FD 9F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
