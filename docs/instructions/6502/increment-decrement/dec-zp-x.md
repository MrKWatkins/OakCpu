# DEC zp,X

Decrements the byte at zero page address zp + X. The zero page address wraps within page zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Set if bit 7 of the result is set, otherwise reset. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if the result is zero, otherwise reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `DEC zp,X` | `D6 zp` | 6 |  |

## References

- [MCS6500 Microcomputer Family Programming Manual](http://archive.6502.org/books/mcs6500_family_programming_manual.pdf)
- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
