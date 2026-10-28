# LD (rr + d), r

Stores r at (rr + d).

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
| `LD (IX + d), B` | `DD 70 d` | 19 |  |
| `LD (IX + d), C` | `DD 71 d` | 19 |  |
| `LD (IX + d), D` | `DD 72 d` | 19 |  |
| `LD (IX + d), E` | `DD 73 d` | 19 |  |
| `LD (IX + d), H` | `DD 74 d` | 19 |  |
| `LD (IX + d), L` | `DD 75 d` | 19 |  |
| `LD (IX + d), A` | `DD 77 d` | 19 |  |
| `LD (IY + d), B` | `FD 70 d` | 19 |  |
| `LD (IY + d), C` | `FD 71 d` | 19 |  |
| `LD (IY + d), D` | `FD 72 d` | 19 |  |
| `LD (IY + d), E` | `FD 73 d` | 19 |  |
| `LD (IY + d), H` | `FD 74 d` | 19 |  |
| `LD (IY + d), L` | `FD 75 d` | 19 |  |
| `LD (IY + d), A` | `FD 77 d` | 19 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
