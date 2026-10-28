# CMP abs,X

Compares A with the byte at address abs + X by subtracting the byte from A, setting the flags from the result and discarding it. Takes an extra cycle if adding X crosses a page.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Set if bit 7 of the result is set, otherwise reset. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if the result is zero, otherwise reset. |
| C | Set if A is greater than or equal to the value read, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `CMP abs,X` | `DD abs` | 4 or 5 |  |

## References

- [MCS6500 Microcomputer Family Programming Manual](http://archive.6502.org/books/mcs6500_family_programming_manual.pdf)
- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
