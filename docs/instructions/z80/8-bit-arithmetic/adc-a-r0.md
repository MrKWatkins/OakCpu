# ADC A, r

Adds r and the carry flag to A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set if there was a carry from bit 3 to bit 4, otherwise reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the addition overflowed, otherwise reset. |
| N | Reset. |
| C | Set if bit 8 of the result is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `ADC A, B` | `88` | 4 |  |
| `ADC A, C` | `89` | 4 |  |
| `ADC A, D` | `8A` | 4 |  |
| `ADC A, E` | `8B` | 4 |  |
| `ADC A, H` | `8C` | 4 |  |
| `ADC A, L` | `8D` | 4 |  |
| `ADC A, A` | `8F` | 4 |  |
| `ADC A, B` | `DD 88` | 8 | Undocumented. |
| `ADC A, C` | `DD 89` | 8 | Undocumented. |
| `ADC A, D` | `DD 8A` | 8 | Undocumented. |
| `ADC A, E` | `DD 8B` | 8 | Undocumented. |
| `ADC A, IXH` | `DD 8C` | 8 | Undocumented. |
| `ADC A, IXL` | `DD 8D` | 8 | Undocumented. |
| `ADC A, A` | `DD 8F` | 8 | Undocumented. |
| `ADC A, B` | `FD 88` | 8 | Undocumented. |
| `ADC A, C` | `FD 89` | 8 | Undocumented. |
| `ADC A, D` | `FD 8A` | 8 | Undocumented. |
| `ADC A, E` | `FD 8B` | 8 | Undocumented. |
| `ADC A, IYH` | `FD 8C` | 8 | Undocumented. |
| `ADC A, IYL` | `FD 8D` | 8 | Undocumented. |
| `ADC A, A` | `FD 8F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
