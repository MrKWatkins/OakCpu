# OTIR

Decrements B, then writes the byte at (HL) to the port addressed by BC and increments HL, repeating until B is zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of B is set, otherwise reset. |
| Z | Set if B is zero, otherwise reset. |
| Y | When the instruction completes, set if bit 1 of B is set, otherwise reset. When it repeats, set if bit 5 of PCH is set, otherwise reset. |
| H | When the instruction completes, set if (the value read + L) is greater than 0xFF, otherwise reset. When it repeats, reset if (the value read + L) is not greater than 0xFF; otherwise set if the low nibble of B is zero when bit 7 of the value read is set, or 0x0F when it is reset, otherwise reset. |
| X | When the instruction completes, set if bit 3 of B is set, otherwise reset. When it repeats, set if bit 3 of PCH is set, otherwise reset. |
| PV | When the instruction completes, set if (((the value read + L) & 7) ^ B) has even parity, otherwise reset. When it repeats, set if (((the value read + L) & 7) ^ B) and (B & 7) have the same parity, otherwise reset; when (the value read + L) is greater than 0xFF, ((B - 1) & 7) is used instead of (B & 7) if bit 7 of the value read is set, and ((B + 1) & 7) if it is reset. |
| N | Set if bit 7 of the value read is set, otherwise reset. |
| C | Set if (the value read + (L & 0xFF)) is greater than 0xFF, otherwise reset. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `OTIR` | `ED B3` | 16 or 21 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
