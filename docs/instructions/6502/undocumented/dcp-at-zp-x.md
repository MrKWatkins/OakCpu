# DCP (zp,X)

Decrements the byte at the address stored at zero page address zp + X, then compares A with the result as CMP does. The zero page address wraps within page zero.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Set if bit 7 of the result is set, otherwise reset. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if the result is zero, otherwise reset. |
| C | Set if A is greater than or equal to the result, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `DCP (zp,X)` | `C3 zp` | 8 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
