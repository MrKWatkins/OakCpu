# AND n

Sets A to the bitwise AND of A and n.

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
| `AND n` | `E6 n` | 7 |  |
| `AND n` | `DD E6 n` | 11 | Undocumented. |
| `AND n` | `FD E6 n` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
