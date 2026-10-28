# LD (nn), rr

Stores rr at address nn, low byte first.

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
| `LD (nn), HL` | `22 nn` | 16 |  |
| `LD (nn), IX` | `DD 22 nn` | 20 |  |
| `LD (nn), BC` | `ED 43 nn` | 20 |  |
| `LD (nn), DE` | `ED 53 nn` | 20 |  |
| `LD (nn), HL` | `ED 63 nn` | 20 | Undocumented. |
| `LD (nn), SP` | `ED 73 nn` | 20 |  |
| `LD (nn), IY` | `FD 22 nn` | 20 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
