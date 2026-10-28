# SRE (zp,X)

Shifts the byte at the address stored at zero page address zp + X right one bit, moving bit 0 into the carry flag, then bitwise exclusive ORs A with the result. The zero page address wraps within page zero.

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
| C | Set if bit 0 of the value read is set, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `SRE (zp,X)` | `43 zp` | 8 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
