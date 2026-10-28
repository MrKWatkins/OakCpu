# RST p

Pushes the address of the next instruction onto the stack, then jumps to address p.

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
| `RST 0x00` | `C7` | 11 |  |
| `RST 0x08` | `CF` | 11 |  |
| `RST 0x10` | `D7` | 11 |  |
| `RST 0x18` | `DF` | 11 |  |
| `RST 0x20` | `E7` | 11 |  |
| `RST 0x28` | `EF` | 11 |  |
| `RST 0x30` | `F7` | 11 |  |
| `RST 0x38` | `FF` | 11 |  |
| `RST 0x00` | `DD C7` | 15 | Undocumented. |
| `RST 0x08` | `DD CF` | 15 | Undocumented. |
| `RST 0x10` | `DD D7` | 15 | Undocumented. |
| `RST 0x18` | `DD DF` | 15 | Undocumented. |
| `RST 0x20` | `DD E7` | 15 | Undocumented. |
| `RST 0x28` | `DD EF` | 15 | Undocumented. |
| `RST 0x30` | `DD F7` | 15 | Undocumented. |
| `RST 0x38` | `DD FF` | 15 | Undocumented. |
| `RST 0x00` | `FD C7` | 15 | Undocumented. |
| `RST 0x08` | `FD CF` | 15 | Undocumented. |
| `RST 0x10` | `FD D7` | 15 | Undocumented. |
| `RST 0x18` | `FD DF` | 15 | Undocumented. |
| `RST 0x20` | `FD E7` | 15 | Undocumented. |
| `RST 0x28` | `FD EF` | 15 | Undocumented. |
| `RST 0x30` | `FD F7` | 15 | Undocumented. |
| `RST 0x38` | `FD FF` | 15 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
