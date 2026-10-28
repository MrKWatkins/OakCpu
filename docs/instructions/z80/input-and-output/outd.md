# OUTD

Decrements B, then writes the byte at (HL) to the port addressed by BC and decrements HL.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of B is set, otherwise reset. |
| Z | Set if B is zero, otherwise reset. |
| Y | Copied from bit 5 of B. |
| H | Set if (the value read + (L & 0xFF)) is greater than 0xFF, otherwise reset. |
| X | Copied from bit 3 of B. |
| PV | Set if (((the value read + (L & 0xFF)) & 0x07) ^ B) has even parity, otherwise reset. |
| N | Set if bit 7 of the value read is set, otherwise reset. |
| C | Set if (the value read + (L & 0xFF)) is greater than 0xFF, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `OUTD` | `ED AB` | 16 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
