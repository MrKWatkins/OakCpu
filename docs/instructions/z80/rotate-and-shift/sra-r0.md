---
cpu: Z80
defined_by: Z80
form: SRA R0
mnemonic: SRA r
group: Rotate and Shift
description: "Shifts {R0} right one bit; bit 0 is copied to the carry flag and bit 7 keeps its value, preserving the sign."
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
  - encoding: CB 28
    mnemonic: SRA B
    operands: { R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 29
    mnemonic: SRA C
    operands: { R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 2A
    mnemonic: SRA D
    operands: { R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 2B
    mnemonic: SRA E
    operands: { R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 2C
    mnemonic: SRA H
    operands: { R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 2D
    mnemonic: SRA L
    operands: { R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 2F
    mnemonic: SRA A
    operands: { R0: A }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SRA r

Shifts r right one bit; bit 0 is copied to the carry flag and bit 7 keeps its value, preserving the sign.

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
| `SRA B` | `CB 28` | 8 |  |
| `SRA C` | `CB 29` | 8 |  |
| `SRA D` | `CB 2A` | 8 |  |
| `SRA E` | `CB 2B` | 8 |  |
| `SRA H` | `CB 2C` | 8 |  |
| `SRA L` | `CB 2D` | 8 |  |
| `SRA A` | `CB 2F` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
