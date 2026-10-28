# DAA

Adjusts A to binary coded decimal after an addition or subtraction of binary coded decimal values, using the N, H and C flags to choose the adjustment.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of A. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of A. |
| H | After an addition, i.e. with the N flag reset, set if the low nibble of A was greater than 9 before the adjustment, otherwise reset. After a subtraction, i.e. with the N flag set, set if the H flag was set and the low nibble of A was less than 6 before the adjustment, otherwise reset. |
| X | Copied from bit 3 of A. |
| PV | Set if A has even parity, otherwise reset. |
| N | Unaffected. |
| C | Set if the C flag was set or A was greater than 0x99 before the adjustment, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `DAA` | `27` | 4 |  |
| `DAA` | `DD 27` | 8 | Undocumented. |
| `DAA` | `FD 27` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
