---
cpu: Z80
defined_by: Z80
form: DJNZ d
mnemonic: DJNZ d
group: Jump
description: Decrements B and, if it is not zero, jumps to the address d bytes from the next instruction.
undocumented: false
flags:
  - flag: S
    effect: unaffected
    description: Unaffected.
  - flag: Z
    effect: unaffected
    description: Unaffected.
  - flag: Y
    effect: unaffected
    description: Unaffected.
  - flag: H
    effect: unaffected
    description: Unaffected.
  - flag: X
    effect: unaffected
    description: Unaffected.
  - flag: PV
    effect: unaffected
    description: Unaffected.
  - flag: N
    effect: unaffected
    description: Unaffected.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: 10 d
    mnemonic: DJNZ d
    tstates: [8, 13]
    undocumented: false
  - encoding: DD 10 d
    mnemonic: DJNZ d
    tstates: [12, 17]
    undocumented: true
  - encoding: FD 10 d
    mnemonic: DJNZ d
    tstates: [12, 17]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# DJNZ d

Decrements B and, if it is not zero, jumps to the address d bytes from the next instruction.

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

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `DJNZ d` | `10 d` | 8 or 13 |  |
| `DJNZ d` | `DD 10 d` | 12 or 17 | Undocumented. |
| `DJNZ d` | `FD 10 d` | 12 or 17 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
