# ALR #n

Bitwise ANDs A with n, then shifts the result right one bit, moving bit 0 into the carry flag.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Reset. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if A is zero, otherwise reset. |
| C | Set if bit 0 of A AND n is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `ALR #n` | `4B n` | 2 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
