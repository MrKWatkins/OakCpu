---
cpu: Z80
defined_by: Z80
form: RL R0
mnemonic: RL r
group: Rotate and Shift
description: "Rotates {R0} left one bit through the carry flag; the carry flag is copied to bit 0 and bit 7 to the carry flag."
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
  - encoding: CB 10
    mnemonic: RL B
    operands: { R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 11
    mnemonic: RL C
    operands: { R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 12
    mnemonic: RL D
    operands: { R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 13
    mnemonic: RL E
    operands: { R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 14
    mnemonic: RL H
    operands: { R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 15
    mnemonic: RL L
    operands: { R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 17
    mnemonic: RL A
    operands: { R0: A }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RL r

Rotates r left one bit through the carry flag; the carry flag is copied to bit 0 and bit 7 to the carry flag.

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
| `RL B` | `CB 10` | 8 |  |
| `RL C` | `CB 11` | 8 |  |
| `RL D` | `CB 12` | 8 |  |
| `RL E` | `CB 13` | 8 |  |
| `RL H` | `CB 14` | 8 |  |
| `RL L` | `CB 15` | 8 |  |
| `RL A` | `CB 17` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
