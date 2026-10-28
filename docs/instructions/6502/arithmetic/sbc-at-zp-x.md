# SBC (zp,X)

Subtracts the byte at the address stored at zero page address zp + X and the borrow, the inverse of the carry flag, from A. In decimal mode the subtraction is binary coded decimal. The zero page address wraps within page zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | In decimal mode, set if bit 7 of the binary result is set, otherwise reset. In binary mode, set if bit 7 of A is set, otherwise reset. |
| V | Set if the subtraction overflowed, otherwise reset. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | In decimal mode, set if the low byte of the binary result is zero, otherwise reset. In binary mode, set if A is zero, otherwise reset. |
| C | Set if bit 8 of the binary result is reset, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `SBC (zp,X)` | `E1 zp` | 6 |  |

## References

- [MCS6500 Microcomputer Family Programming Manual](http://archive.6502.org/books/mcs6500_family_programming_manual.pdf)
- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
