# LAS abs,Y

Loads A, X and S with the byte at address abs + Y AND S. Takes an extra cycle if adding Y crosses a page.

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
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | Cycles | Notes |
| ----------- | ------ | ------ | ----- |
| `LAS abs,Y` | `BB abs` | 4 or 5 |  |

## References

- [6502 Instruction Set, mass:werk](https://www.masswerk.at/6502/6502_instruction_set.html)
- [CPU unofficial opcodes, NESdev Wiki](https://www.nesdev.org/wiki/CPU_unofficial_opcodes)
