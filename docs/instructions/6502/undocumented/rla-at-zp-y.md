# RLA (zp),Y

Rotates the byte at Y plus the address stored at zero page address zp left one bit through the carry flag, then bitwise ANDs A with the result.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | Set if bit 7 of A is set, otherwise reset. |
| V | Unaffected. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | Set if A is zero, otherwise reset. |
| C | Set if bit 7 of the value read is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `RLA (zp),Y` | `33 zp` | 8 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
