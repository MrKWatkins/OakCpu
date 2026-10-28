# CP r

Compares A with r by subtracting r from A, setting the flags from the result and leaving A unchanged.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Set if there was a borrow from bit 4, otherwise reset. |
| X | Copied from bit 3 of r. |
| PV | Set if the subtraction overflowed, otherwise reset. |
| N | Set. |
| C | Set if bit 8 of the result is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `CP B` | `B8` | 4 |  |
| `CP C` | `B9` | 4 |  |
| `CP D` | `BA` | 4 |  |
| `CP E` | `BB` | 4 |  |
| `CP H` | `BC` | 4 |  |
| `CP L` | `BD` | 4 |  |
| `CP A` | `BF` | 4 |  |
| `CP B` | `DD B8` | 8 | Undocumented. |
| `CP C` | `DD B9` | 8 | Undocumented. |
| `CP D` | `DD BA` | 8 | Undocumented. |
| `CP E` | `DD BB` | 8 | Undocumented. |
| `CP IXH` | `DD BC` | 8 | Undocumented. |
| `CP IXL` | `DD BD` | 8 | Undocumented. |
| `CP A` | `DD BF` | 8 | Undocumented. |
| `CP B` | `FD B8` | 8 | Undocumented. |
| `CP C` | `FD B9` | 8 | Undocumented. |
| `CP D` | `FD BA` | 8 | Undocumented. |
| `CP E` | `FD BB` | 8 | Undocumented. |
| `CP IYH` | `FD BC` | 8 | Undocumented. |
| `CP IYL` | `FD BD` | 8 | Undocumented. |
| `CP A` | `FD BF` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
