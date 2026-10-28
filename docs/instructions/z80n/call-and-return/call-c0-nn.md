# CALL cc, nn

If condition cc is met, pushes the address of the next instruction onto the stack and jumps to nn; otherwise continues with the next instruction.

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
| `CALL NZ, nn` | `C4 nn` | 10 or 17 |  |
| `CALL Z, nn` | `CC nn` | 10 or 17 |  |
| `CALL NC, nn` | `D4 nn` | 10 or 17 |  |
| `CALL C, nn` | `DC nn` | 10 or 17 |  |
| `CALL PO, nn` | `E4 nn` | 10 or 17 |  |
| `CALL PE, nn` | `EC nn` | 10 or 17 |  |
| `CALL P, nn` | `F4 nn` | 10 or 17 |  |
| `CALL M, nn` | `FC nn` | 10 or 17 |  |
| `CALL NZ, nn` | `DD C4 nn` | 14 or 21 | Undocumented. |
| `CALL Z, nn` | `DD CC nn` | 14 or 21 | Undocumented. |
| `CALL NC, nn` | `DD D4 nn` | 14 or 21 | Undocumented. |
| `CALL C, nn` | `DD DC nn` | 14 or 21 | Undocumented. |
| `CALL PO, nn` | `DD E4 nn` | 14 or 21 | Undocumented. |
| `CALL PE, nn` | `DD EC nn` | 14 or 21 | Undocumented. |
| `CALL P, nn` | `DD F4 nn` | 14 or 21 | Undocumented. |
| `CALL M, nn` | `DD FC nn` | 14 or 21 | Undocumented. |
| `CALL NZ, nn` | `FD C4 nn` | 14 or 21 | Undocumented. |
| `CALL Z, nn` | `FD CC nn` | 14 or 21 | Undocumented. |
| `CALL NC, nn` | `FD D4 nn` | 14 or 21 | Undocumented. |
| `CALL C, nn` | `FD DC nn` | 14 or 21 | Undocumented. |
| `CALL PO, nn` | `FD E4 nn` | 14 or 21 | Undocumented. |
| `CALL PE, nn` | `FD EC nn` | 14 or 21 | Undocumented. |
| `CALL P, nn` | `FD F4 nn` | 14 or 21 | Undocumented. |
| `CALL M, nn` | `FD FC nn` | 14 or 21 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
