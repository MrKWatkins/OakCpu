---
cpu: Z80N
defined_by: Z80
form: IM 0
mnemonic: IM 0
group: General-Purpose Arithmetic and CPU Control
description: Selects interrupt mode 0, in which the interrupting device supplies an instruction on the data bus for the CPU to execute.
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
  - encoding: ED 46
    mnemonic: IM 0
    tstates: [8]
    undocumented: false
  - encoding: ED 4E
    mnemonic: IM 0
    tstates: [8]
    undocumented: true
  - encoding: ED 66
    mnemonic: IM 0
    tstates: [8]
    undocumented: true
  - encoding: ED 6E
    mnemonic: IM 0
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# IM 0

Selects interrupt mode 0, in which the interrupting device supplies an instruction on the data bus for the CPU to execute.

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
| `IM 0` | `ED 46` | 8 |  |
| `IM 0` | `ED 4E` | 8 | Undocumented. |
| `IM 0` | `ED 66` | 8 | Undocumented. |
| `IM 0` | `ED 6E` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
