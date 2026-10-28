# ARR #n

Bitwise ANDs A with n, then rotates the result right one bit through the carry flag. The carry and overflow flags are set from bits 6 and 5 of the result as if by an addition, and in decimal mode the result is adjusted as it would be by ADC.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Set if bit 7 of A is set, otherwise reset. |
| V | Set if bits 6 and 5 of the result differ, otherwise reset. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if A is zero, otherwise reset. |
| C | In decimal mode, set if the high nibble of A AND n, plus 0x10 if bit 4 of A AND n is set, is greater than 0x50, otherwise reset. In binary mode, set if bit 6 of the result is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `ARR #n` | `6B n` | 2 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
