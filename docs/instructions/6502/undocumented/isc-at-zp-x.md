# ISC (zp,X)

Increments the byte at the address stored at zero page address zp + X, then subtracts the result and the borrow from A as SBC does. The zero page address wraps within page zero.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| N | In decimal mode, set if bit 7 of the binary result is set, otherwise reset. In binary mode, set if bit 7 of A is set, otherwise reset. |
| V | Set if the subtraction overflowed, otherwise reset. |
| B | Unaffected. |
| D | Unaffected. |
| I | Unaffected. |
| Z | In decimal mode, set if the low byte of the binary result is zero, otherwise reset. In binary mode, set if A is zero, otherwise reset. |
| C | Set if bit 8 of the binary result is reset, otherwise reset. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `ISC (zp,X)` | `E3 zp` | 8 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
