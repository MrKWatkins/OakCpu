# LDX abs,Y

Loads X with the byte at address abs + Y. Takes an extra cycle if adding Y crosses a page.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Set if bit 7 of X is set, otherwise reset. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if X is zero, otherwise reset. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `LDX abs,Y` | `BE abs` | 4 or 5 |  |

## References

- [MCS6500 Microcomputer Family Programming Manual](http://archive.6502.org/books/mcs6500_family_programming_manual.pdf)
- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
