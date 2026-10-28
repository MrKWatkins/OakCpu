---
cpu: Z80
defined_by: Z80
form: RLC R0
mnemonic: RLC r
group: Rotate and Shift
description: "Rotates {R0} left one bit; bit 7 is copied to both bit 0 and the carry flag."
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
    description: "Set if {R0} has even parity, otherwise reset."
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: changed
    description: "Set if bit 7 of the original value of {R0} is set, otherwise reset."
opcodes:
  - encoding: CB 00
    mnemonic: RLC B
    operands: { R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 01
    mnemonic: RLC C
    operands: { R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 02
    mnemonic: RLC D
    operands: { R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 03
    mnemonic: RLC E
    operands: { R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 04
    mnemonic: RLC H
    operands: { R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 05
    mnemonic: RLC L
    operands: { R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 07
    mnemonic: RLC A
    operands: { R0: A }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RLC r

Rotates r left one bit; bit 7 is copied to both bit 0 and the carry flag.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of r. |
| Z | Set if r is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Reset. |
| X | Copied from bit 3 of r. |
| PV | Set if r has even parity, otherwise reset. |
| N | Reset. |
| C | Set if bit 7 of the original value of r is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `RLC B` | `CB 00` | 8 |  |
| `RLC C` | `CB 01` | 8 |  |
| `RLC D` | `CB 02` | 8 |  |
| `RLC E` | `CB 03` | 8 |  |
| `RLC H` | `CB 04` | 8 |  |
| `RLC L` | `CB 05` | 8 |  |
| `RLC A` | `CB 07` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
