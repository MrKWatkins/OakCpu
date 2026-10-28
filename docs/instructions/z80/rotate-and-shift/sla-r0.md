---
cpu: Z80
defined_by: Z80
form: SLA R0
mnemonic: SLA r
group: Rotate and Shift
description: "Shifts {R0} left one bit; bit 7 is copied to the carry flag and bit 0 is reset."
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
  - encoding: CB 20
    mnemonic: SLA B
    operands: { R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 21
    mnemonic: SLA C
    operands: { R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 22
    mnemonic: SLA D
    operands: { R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 23
    mnemonic: SLA E
    operands: { R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 24
    mnemonic: SLA H
    operands: { R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 25
    mnemonic: SLA L
    operands: { R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 27
    mnemonic: SLA A
    operands: { R0: A }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SLA r

Shifts r left one bit; bit 7 is copied to the carry flag and bit 0 is reset.

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
| `SLA B` | `CB 20` | 8 |  |
| `SLA C` | `CB 21` | 8 |  |
| `SLA D` | `CB 22` | 8 |  |
| `SLA E` | `CB 23` | 8 |  |
| `SLA H` | `CB 24` | 8 |  |
| `SLA L` | `CB 25` | 8 |  |
| `SLA A` | `CB 27` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
