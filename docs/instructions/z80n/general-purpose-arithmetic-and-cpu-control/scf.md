---
cpu: Z80N
defined_by: Z80
form: SCF
mnemonic: SCF
group: General-Purpose Arithmetic and CPU Control
description: Sets the carry flag.
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
    effect: reset
    description: Reset.
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
    effect: set
    description: Set.
opcodes:
  - encoding: 37
    mnemonic: SCF
    tstates: [4]
    undocumented: false
  - encoding: DD 37
    mnemonic: SCF
    tstates: [8]
    undocumented: true
  - encoding: FD 37
    mnemonic: SCF
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SCF

Sets the carry flag.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Copied from bit 5 of ((Q ^ F) | A). |
| H | Reset. |
| X | Copied from bit 3 of ((Q ^ F) | A). |
| PV | Unaffected. |
| N | Reset. |
| C | Set. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `SCF` | `37` | 4 |  |
| `SCF` | `DD 37` | 8 | Undocumented. |
| `SCF` | `FD 37` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
