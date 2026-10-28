# JP cc, nn

Jumps to nn if condition cc is met; otherwise continues with the next instruction.

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
| `JP NZ, nn` | `C2 nn` | 10 |  |
| `JP Z, nn` | `CA nn` | 10 |  |
| `JP NC, nn` | `D2 nn` | 10 |  |
| `JP C, nn` | `DA nn` | 10 |  |
| `JP PO, nn` | `E2 nn` | 10 |  |
| `JP PE, nn` | `EA nn` | 10 |  |
| `JP P, nn` | `F2 nn` | 10 |  |
| `JP M, nn` | `FA nn` | 10 |  |
| `JP NZ, nn` | `DD C2 nn` | 14 | Undocumented. |
| `JP Z, nn` | `DD CA nn` | 14 | Undocumented. |
| `JP NC, nn` | `DD D2 nn` | 14 | Undocumented. |
| `JP C, nn` | `DD DA nn` | 14 | Undocumented. |
| `JP PO, nn` | `DD E2 nn` | 14 | Undocumented. |
| `JP PE, nn` | `DD EA nn` | 14 | Undocumented. |
| `JP P, nn` | `DD F2 nn` | 14 | Undocumented. |
| `JP M, nn` | `DD FA nn` | 14 | Undocumented. |
| `JP NZ, nn` | `FD C2 nn` | 14 | Undocumented. |
| `JP Z, nn` | `FD CA nn` | 14 | Undocumented. |
| `JP NC, nn` | `FD D2 nn` | 14 | Undocumented. |
| `JP C, nn` | `FD DA nn` | 14 | Undocumented. |
| `JP PO, nn` | `FD E2 nn` | 14 | Undocumented. |
| `JP PE, nn` | `FD EA nn` | 14 | Undocumented. |
| `JP P, nn` | `FD F2 nn` | 14 | Undocumented. |
| `JP M, nn` | `FD FA nn` | 14 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
