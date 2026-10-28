# SUB n

Subtracts n from A.

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
| `SUB n` | `D6 n` | 7 |  |
| `SUB n` | `DD D6 n` | 11 | Undocumented. |
| `SUB n` | `FD D6 n` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
