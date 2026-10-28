# LDA zp,X

Loads A with the byte at zero page address zp + X. The zero page address wraps within page zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Set if bit 7 of A is set, otherwise reset. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if A is zero, otherwise reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `LDA zp,X` | `B5 zp` | 4 |  |

## References

- [MCS6500 Microcomputer Family Programming Manual](http://archive.6502.org/books/mcs6500_family_programming_manual.pdf)
- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
