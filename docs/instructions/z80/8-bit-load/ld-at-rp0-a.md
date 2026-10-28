# LD (rr), A

Stores A at (rr).

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
| `LD (BC), A` | `02` | 7 |  |
| `LD (DE), A` | `12` | 7 |  |
| `LD (BC), A` | `DD 02` | 11 | Undocumented. |
| `LD (DE), A` | `DD 12` | 11 | Undocumented. |
| `LD (BC), A` | `FD 02` | 11 | Undocumented. |
| `LD (DE), A` | `FD 12` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
