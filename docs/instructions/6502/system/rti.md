# RTI

Pops the processor status and then the return address from the stack, returning from an interrupt.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Copied from bit 7 of the value read. |
| V | Copied from bit 6 of the value read. |
| B | Reset. |
| D | Copied from bit 3 of the value read. |
| I | Copied from bit 2 of the value read. |
| Z | Copied from bit 1 of the value read. |
| C | Copied from bit 0 of the value read. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `RTI` | `40` | 6 |  |

## References

- [MCS6500 Microcomputer Family Programming Manual](http://archive.6502.org/books/mcs6500_family_programming_manual.pdf)
- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
