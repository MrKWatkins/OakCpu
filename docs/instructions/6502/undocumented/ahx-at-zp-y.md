# AHX (zp),Y

Stores A AND X AND the high byte of the address stored at zp plus one at Y plus the address stored at zero page address zp. If adding Y crosses a page, the high byte of the address written is replaced by the value stored.

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
| `AHX (zp),Y` | `93 zp` | 6 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
