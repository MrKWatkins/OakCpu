---
cpu: Z80
defined_by: Z80
form: CCF
mnemonic: CCF
group: General-Purpose Arithmetic and CPU Control
description: Inverts the carry flag.
undocumented: false
flags:
  - flag: S
    effect: unaffected
    description: Unaffected.
  - flag: Z
    effect: unaffected
    description: Unaffected.
  - flag: Y
    effect: changed
    description: "Copied from bit 5 of ((Q ^ F) | A)."
  - flag: H
    effect: changed
    description: Set to the previous value of the C flag.
  - flag: X
    effect: changed
    description: "Copied from bit 3 of ((Q ^ F) | A)."
  - flag: PV
    effect: unaffected
    description: Unaffected.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: changed
    description: Inverted.
opcodes:
  - encoding: 3F
    mnemonic: CCF
    tstates: [4]
    undocumented: false
  - encoding: DD 3F
    mnemonic: CCF
    tstates: [8]
    undocumented: true
  - encoding: FD 3F
    mnemonic: CCF
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# CCF

Inverts the carry flag.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Copied from bit 5 of ((Q ^ F) | A). |
| H | Set to the previous value of the C flag. |
| X | Copied from bit 3 of ((Q ^ F) | A). |
| PV | Unaffected. |
| N | Reset. |
| C | Inverted. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `CCF` | `3F` | 4 |  |
| `CCF` | `DD 3F` | 8 | Undocumented. |
| `CCF` | `FD 3F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
