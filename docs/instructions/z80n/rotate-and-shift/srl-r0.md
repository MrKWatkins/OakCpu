---
cpu: Z80N
defined_by: Z80
form: SRL R0
mnemonic: SRL r
group: Rotate and Shift
description: "Shifts {R0} right one bit; bit 0 is copied to the carry flag and bit 7 is reset."
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
  - encoding: CB 38
    mnemonic: SRL B
    operands: { R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 39
    mnemonic: SRL C
    operands: { R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 3A
    mnemonic: SRL D
    operands: { R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 3B
    mnemonic: SRL E
    operands: { R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 3C
    mnemonic: SRL H
    operands: { R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 3D
    mnemonic: SRL L
    operands: { R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 3F
    mnemonic: SRL A
    operands: { R0: A }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SRL r

Shifts r right one bit; bit 0 is copied to the carry flag and bit 7 is reset.

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
| `SRL B` | `CB 38` | 8 |  |
| `SRL C` | `CB 39` | 8 |  |
| `SRL D` | `CB 3A` | 8 |  |
| `SRL E` | `CB 3B` | 8 |  |
| `SRL H` | `CB 3C` | 8 |  |
| `SRL L` | `CB 3D` | 8 |  |
| `SRL A` | `CB 3F` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
