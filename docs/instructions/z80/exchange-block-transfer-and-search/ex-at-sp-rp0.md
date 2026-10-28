# EX (SP), rr

Exchanges rr with the word at the top of the stack, its low byte with (SP) and its high byte with (SP + 1).

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
| `EX (SP), HL` | `E3` | 19 |  |
| `EX (SP), IX` | `DD E3` | 23 |  |
| `EX (SP), IY` | `FD E3` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
