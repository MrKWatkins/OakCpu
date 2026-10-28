---
cpu: Z80N
defined_by: Z80
form: EX AF, AF'
mnemonic: EX AF, AF'
group: Exchange, Block Transfer, and Search
description: Exchanges AF with its shadow register AF'.
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
  - encoding: 08
    mnemonic: EX AF, AF'
    tstates: [4]
    undocumented: false
  - encoding: DD 08
    mnemonic: EX AF, AF'
    tstates: [8]
    undocumented: true
  - encoding: FD 08
    mnemonic: EX AF, AF'
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# EX AF, AF'

Exchanges AF with its shadow register AF'.

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
| `EX AF, AF'` | `08` | 4 |  |
| `EX AF, AF'` | `DD 08` | 8 | Undocumented. |
| `EX AF, AF'` | `FD 08` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
