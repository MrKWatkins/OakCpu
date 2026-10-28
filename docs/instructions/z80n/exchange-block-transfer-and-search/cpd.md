# CPD

Compares A with the byte at (HL) by subtraction, setting the flags from the result, then decrements HL and BC.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Set if bit 1 of (the result - the half borrow) is set, otherwise reset. |
| H | Set if there was a borrow from bit 4, otherwise reset. |
| X | Set if bit 3 of (the result - the half borrow) is set, otherwise reset. |
| PV | Set if BC is not zero, otherwise reset. |
| N | Set. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `CPD` | `ED A9` | 16 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
