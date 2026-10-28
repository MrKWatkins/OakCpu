---
cpu: Z80
defined_by: Z80
form: RR R0
mnemonic: RR r
group: Rotate and Shift
description: "Rotates {R0} right one bit through the carry flag; the carry flag is copied to bit 7 and bit 0 to the carry flag."
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
  - encoding: CB 18
    mnemonic: RR B
    operands: { R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 19
    mnemonic: RR C
    operands: { R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 1A
    mnemonic: RR D
    operands: { R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 1B
    mnemonic: RR E
    operands: { R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 1C
    mnemonic: RR H
    operands: { R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 1D
    mnemonic: RR L
    operands: { R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 1F
    mnemonic: RR A
    operands: { R0: A }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RR r

Rotates r right one bit through the carry flag; the carry flag is copied to bit 7 and bit 0 to the carry flag.

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
| `RR B` | `CB 18` | 8 |  |
| `RR C` | `CB 19` | 8 |  |
| `RR D` | `CB 1A` | 8 |  |
| `RR E` | `CB 1B` | 8 |  |
| `RR H` | `CB 1C` | 8 |  |
| `RR L` | `CB 1D` | 8 |  |
| `RR A` | `CB 1F` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
