---
cpu: Z80
defined_by: Z80
form: CPL
mnemonic: CPL
group: General-Purpose Arithmetic and CPU Control
description: Inverts every bit of A.
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
    description: Copied from bit 5 of A.
  - flag: H
    effect: set
    description: Set.
  - flag: X
    effect: changed
    description: Copied from bit 3 of A.
  - flag: PV
    effect: unaffected
    description: Unaffected.
  - flag: N
    effect: set
    description: Set.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: 2F
    mnemonic: CPL
    tstates: [4]
    undocumented: false
  - encoding: DD 2F
    mnemonic: CPL
    tstates: [8]
    undocumented: true
  - encoding: FD 2F
    mnemonic: CPL
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# CPL

Inverts every bit of A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Copied from bit 5 of A. |
| H | Set. |
| X | Copied from bit 3 of A. |
| PV | Unaffected. |
| N | Set. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `CPL` | `2F` | 4 |  |
| `CPL` | `DD 2F` | 8 | Undocumented. |
| `CPL` | `FD 2F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
