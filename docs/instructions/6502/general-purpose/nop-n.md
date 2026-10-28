# NOP #n

Does nothing; the operand byte is read and ignored.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Unaffected. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Unaffected. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `NOP #n` | `80 n` | 2 |  |
| `NOP #n` | `82 n` | 2 |  |
| `NOP #n` | `89 n` | 2 |  |
| `NOP #n` | `C2 n` | 2 |  |
| `NOP #n` | `E2 n` | 2 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
