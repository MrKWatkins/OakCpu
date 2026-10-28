---
cpu: Z80
defined_by: Z80
form: LD A, R0
mnemonic: LD A, r
group: 8-Bit Load
description: "Copies {R0} into A."
undocumented: false
flags:
  - flag: S
    effect: changed
    description: "Copied from bit 7 of {R0}."
  - flag: Z
    effect: changed
    description: "Set if {R0} is zero, otherwise reset."
  - flag: Y
    effect: changed
    description: "Copied from bit 5 of {R0}."
  - flag: H
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: "Copied from bit 3 of {R0}."
  - flag: PV
    effect: changed
    description: Set to the value of IFF2.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: ED 57
    mnemonic: LD A, I
    operands: { R0: I }
    tstates: [9]
    undocumented: false
  - encoding: ED 5F
    mnemonic: LD A, R
    operands: { R0: R }
    tstates: [9]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD A, r

Copies r into A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of r. |
| Z | Set if r is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Reset. |
| X | Copied from bit 3 of r. |
| PV | Set to the value of IFF2. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `LD A, I` | `ED 57` | 9 |  |
| `LD A, R` | `ED 5F` | 9 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
