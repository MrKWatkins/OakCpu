# RRA abs,X

Rotates the byte at address abs + X right one bit through the carry flag, then adds the result and the new carry flag to A as ADC does.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | In decimal mode, set if bit 7 of the sum of the high nibbles, including any carry from the low nibbles but before the decimal adjustment, is set, otherwise reset. In binary mode, set if bit 7 of A is set, otherwise reset. |
| V | Set if the addition overflowed, otherwise reset. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | In decimal mode, set if the low byte of the binary result is zero, otherwise reset. In binary mode, set if A is zero, otherwise reset. |
| C | In decimal mode, set if the decimal result is greater than 99, otherwise reset. In binary mode, set if bit 8 of the binary result is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `RRA abs,X` | `7F abs` | 7 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
