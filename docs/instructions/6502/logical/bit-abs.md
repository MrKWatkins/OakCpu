# BIT abs

Tests A against the byte at address abs: the Z flag is set from A AND the byte, and the N and V flags are copied from bits 7 and 6 of the byte.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Set if bit 7 of the value read is set, otherwise reset. |
| V | Set if bit 6 of the value read is set, otherwise reset. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if the result is zero, otherwise reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `BIT abs` | `2C abs` | 4 |  |

## References

- [MCS6500 Microcomputer Family Programming Manual](http://archive.6502.org/books/mcs6500_family_programming_manual.pdf)
- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
