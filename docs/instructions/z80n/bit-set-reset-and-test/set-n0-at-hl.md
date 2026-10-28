# SET b, (HL)

Sets bit b of the byte at (HL).

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
| `SET 0, (HL)` | `CB C6` | 15 |  |
| `SET 1, (HL)` | `CB CE` | 15 |  |
| `SET 2, (HL)` | `CB D6` | 15 |  |
| `SET 3, (HL)` | `CB DE` | 15 |  |
| `SET 4, (HL)` | `CB E6` | 15 |  |
| `SET 5, (HL)` | `CB EE` | 15 |  |
| `SET 6, (HL)` | `CB F6` | 15 |  |
| `SET 7, (HL)` | `CB FE` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
