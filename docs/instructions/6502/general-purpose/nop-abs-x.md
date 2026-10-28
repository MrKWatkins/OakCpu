# NOP abs,X

Does nothing; the byte at address abs + X is read and ignored. Takes an extra cycle if adding X crosses a page.

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
| `NOP abs,X` | `1C abs` | 4 or 5 |  |
| `NOP abs,X` | `3C abs` | 4 or 5 |  |
| `NOP abs,X` | `5C abs` | 4 or 5 |  |
| `NOP abs,X` | `7C abs` | 4 or 5 |  |
| `NOP abs,X` | `DC abs` | 4 or 5 |  |
| `NOP abs,X` | `FC abs` | 4 or 5 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
