# LD (HL), r

Stores r at (HL).

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
| `LD (HL), B` | `70` | 7 |  |
| `LD (HL), C` | `71` | 7 |  |
| `LD (HL), D` | `72` | 7 |  |
| `LD (HL), E` | `73` | 7 |  |
| `LD (HL), H` | `74` | 7 |  |
| `LD (HL), L` | `75` | 7 |  |
| `LD (HL), A` | `77` | 7 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
