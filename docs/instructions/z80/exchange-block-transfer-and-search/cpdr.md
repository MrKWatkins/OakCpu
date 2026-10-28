# CPDR

Compares A with the byte at (HL) by subtraction, setting the flags from the result, then decrements HL and BC, repeating until A matches the byte or BC is zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of latch. |
| Z | Set if latch is zero, otherwise reset. |
| Y | When the instruction completes, set if bit 1 of (latch - the half borrow) is set, otherwise reset. When it repeats, set if bit 5 of PCH is set, otherwise reset. |
| H | Set if there was a borrow from bit 4, otherwise reset. |
| X | When the instruction completes, set if bit 3 of (latch - the half borrow) is set, otherwise reset. When it repeats, set if bit 3 of PCH is set, otherwise reset. |
| PV | Set if BC is not zero, otherwise reset. |
| N | Set. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `CPDR` | `ED B9` | 16 or 21 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
