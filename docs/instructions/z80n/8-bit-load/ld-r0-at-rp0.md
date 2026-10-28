# LD r, (rr)

Loads the byte at (rr) into r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Unaffected. |
| H | Unaffected. |
| X | Unaffected. |
| PV | Unaffected. |
| N | Unaffected. |
| C | Unaffected. |

## Opcodes

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `LD B, (HL)` | `46` | 7 |  |
| `LD C, (HL)` | `4E` | 7 |  |
| `LD D, (HL)` | `56` | 7 |  |
| `LD E, (HL)` | `5E` | 7 |  |
| `LD H, (HL)` | `66` | 7 |  |
| `LD L, (HL)` | `6E` | 7 |  |
| `LD A, (HL)` | `7E` | 7 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
