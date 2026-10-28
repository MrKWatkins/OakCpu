# SBX #n

Subtracts n from A AND X, without the borrow, and stores the result in X.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Set if bit 7 of X is set, otherwise reset. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if X is zero, otherwise reset. |
| C | Set if A AND X is greater than or equal to n, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `SBX #n` | `CB n` | 2 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
