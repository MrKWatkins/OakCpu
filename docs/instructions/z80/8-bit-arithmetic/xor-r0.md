# XOR r

Sets A to the bitwise exclusive OR of A and r.

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
| `XOR B` | `A8` | 4 |  |
| `XOR C` | `A9` | 4 |  |
| `XOR D` | `AA` | 4 |  |
| `XOR E` | `AB` | 4 |  |
| `XOR H` | `AC` | 4 |  |
| `XOR L` | `AD` | 4 |  |
| `XOR A` | `AF` | 4 |  |
| `XOR B` | `DD A8` | 8 | Undocumented. |
| `XOR C` | `DD A9` | 8 | Undocumented. |
| `XOR D` | `DD AA` | 8 | Undocumented. |
| `XOR E` | `DD AB` | 8 | Undocumented. |
| `XOR IXH` | `DD AC` | 8 | Undocumented. |
| `XOR IXL` | `DD AD` | 8 | Undocumented. |
| `XOR A` | `DD AF` | 8 | Undocumented. |
| `XOR B` | `FD A8` | 8 | Undocumented. |
| `XOR C` | `FD A9` | 8 | Undocumented. |
| `XOR D` | `FD AA` | 8 | Undocumented. |
| `XOR E` | `FD AB` | 8 | Undocumented. |
| `XOR IYH` | `FD AC` | 8 | Undocumented. |
| `XOR IYL` | `FD AD` | 8 | Undocumented. |
| `XOR A` | `FD AF` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
