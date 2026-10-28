---
cpu: Z80N
defined_by: Z80
form: RRC R0
mnemonic: RRC r
group: Rotate and Shift
description: "Rotates {R0} right one bit; bit 0 is copied to both bit 7 and the carry flag."
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
    description: "Copied from bit 0 of the original value of {R0}."
opcodes:
  - encoding: CB 08
    mnemonic: RRC B
    operands: { R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 09
    mnemonic: RRC C
    operands: { R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 0A
    mnemonic: RRC D
    operands: { R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 0B
    mnemonic: RRC E
    operands: { R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 0C
    mnemonic: RRC H
    operands: { R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 0D
    mnemonic: RRC L
    operands: { R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 0F
    mnemonic: RRC A
    operands: { R0: A }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RRC r

Rotates r right one bit; bit 0 is copied to both bit 7 and the carry flag.

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
| C | Copied from bit 0 of the original value of r. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `RRC B` | `CB 08` | 8 |  |
| `RRC C` | `CB 09` | 8 |  |
| `RRC D` | `CB 0A` | 8 |  |
| `RRC E` | `CB 0B` | 8 |  |
| `RRC H` | `CB 0C` | 8 |  |
| `RRC L` | `CB 0D` | 8 |  |
| `RRC A` | `CB 0F` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
