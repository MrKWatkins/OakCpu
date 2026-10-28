# LSR abs,X

Shifts the byte at address abs + X right one bit, moving bit 0 into the carry flag and resetting bit 7.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Reset. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if the result is zero, otherwise reset. |
| C | Set if bit 0 of the value read is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `LSR abs,X` | `5E abs` | 7 |  |

## References

- [MCS6500 Microcomputer Family Programming Manual](http://archive.6502.org/books/mcs6500_family_programming_manual.pdf)
- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
