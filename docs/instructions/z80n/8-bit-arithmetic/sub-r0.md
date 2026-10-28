# SUB r

Subtracts r from A.

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
| `SUB B` | `90` | 4 |  |
| `SUB C` | `91` | 4 |  |
| `SUB D` | `92` | 4 |  |
| `SUB E` | `93` | 4 |  |
| `SUB H` | `94` | 4 |  |
| `SUB L` | `95` | 4 |  |
| `SUB A` | `97` | 4 |  |
| `SUB B` | `DD 90` | 8 | Undocumented. |
| `SUB C` | `DD 91` | 8 | Undocumented. |
| `SUB D` | `DD 92` | 8 | Undocumented. |
| `SUB E` | `DD 93` | 8 | Undocumented. |
| `SUB IXH` | `DD 94` | 8 | Undocumented. |
| `SUB IXL` | `DD 95` | 8 | Undocumented. |
| `SUB A` | `DD 97` | 8 | Undocumented. |
| `SUB B` | `FD 90` | 8 | Undocumented. |
| `SUB C` | `FD 91` | 8 | Undocumented. |
| `SUB D` | `FD 92` | 8 | Undocumented. |
| `SUB E` | `FD 93` | 8 | Undocumented. |
| `SUB IYH` | `FD 94` | 8 | Undocumented. |
| `SUB IYL` | `FD 95` | 8 | Undocumented. |
| `SUB A` | `FD 97` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
