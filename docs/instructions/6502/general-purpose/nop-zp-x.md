# NOP zp,X

Does nothing; the byte at zero page address zp + X is read and ignored. The zero page address wraps within page zero.

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
| `NOP zp,X` | `14 zp` | 4 |  |
| `NOP zp,X` | `34 zp` | 4 |  |
| `NOP zp,X` | `54 zp` | 4 |  |
| `NOP zp,X` | `74 zp` | 4 |  |
| `NOP zp,X` | `D4 zp` | 4 |  |
| `NOP zp,X` | `F4 zp` | 4 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
