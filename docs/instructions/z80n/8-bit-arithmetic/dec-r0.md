---
cpu: Z80N
defined_by: Z80
form: DEC R0
mnemonic: DEC r
group: 8-Bit Arithmetic
description: "Decrements {R0}."
undocumented: false
flags:
  - flag: S
    effect: changed
    description: "Set if bit 7 of {R0} is set, otherwise reset."
  - flag: Z
    effect: changed
    description: "Set if {R0} is zero, otherwise reset."
  - flag: Y
    effect: changed
    description: "Copied from bit 5 of {R0}."
  - flag: H
    effect: changed
    description: "Set if ({R0} & 0x0F) equals 0x0F, otherwise reset."
  - flag: X
    effect: changed
    description: "Copied from bit 3 of {R0}."
  - flag: PV
    effect: changed
    description: "Set if {R0} equals 0x7F, otherwise reset."
  - flag: N
    effect: set
    description: Set.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: 05
    mnemonic: DEC B
    operands: { R0: B }
    tstates: [4]
    undocumented: false
  - encoding: 0D
    mnemonic: DEC C
    operands: { R0: C }
    tstates: [4]
    undocumented: false
  - encoding: 15
    mnemonic: DEC D
    operands: { R0: D }
    tstates: [4]
    undocumented: false
  - encoding: 1D
    mnemonic: DEC E
    operands: { R0: E }
    tstates: [4]
    undocumented: false
  - encoding: 25
    mnemonic: DEC H
    operands: { R0: H }
    tstates: [4]
    undocumented: false
  - encoding: 2D
    mnemonic: DEC L
    operands: { R0: L }
    tstates: [4]
    undocumented: false
  - encoding: 3D
    mnemonic: DEC A
    operands: { R0: A }
    tstates: [4]
    undocumented: false
  - encoding: DD 05
    mnemonic: DEC B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 0D
    mnemonic: DEC C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 15
    mnemonic: DEC D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 1D
    mnemonic: DEC E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 25
    mnemonic: DEC IXH
    operands: { R0: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 2D
    mnemonic: DEC IXL
    operands: { R0: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 3D
    mnemonic: DEC A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 05
    mnemonic: DEC B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 0D
    mnemonic: DEC C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 15
    mnemonic: DEC D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 1D
    mnemonic: DEC E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 25
    mnemonic: DEC IYH
    operands: { R0: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 2D
    mnemonic: DEC IYL
    operands: { R0: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 3D
    mnemonic: DEC A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# DEC r

Decrements r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of r is set, otherwise reset. |
| Z | Set if r is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Set if (r & 0x0F) equals 0x0F, otherwise reset. |
| X | Copied from bit 3 of r. |
| PV | Set if r equals 0x7F, otherwise reset. |
| N | Set. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `DEC B` | `05` | 4 |  |
| `DEC C` | `0D` | 4 |  |
| `DEC D` | `15` | 4 |  |
| `DEC E` | `1D` | 4 |  |
| `DEC H` | `25` | 4 |  |
| `DEC L` | `2D` | 4 |  |
| `DEC A` | `3D` | 4 |  |
| `DEC B` | `DD 05` | 8 | Undocumented. |
| `DEC C` | `DD 0D` | 8 | Undocumented. |
| `DEC D` | `DD 15` | 8 | Undocumented. |
| `DEC E` | `DD 1D` | 8 | Undocumented. |
| `DEC IXH` | `DD 25` | 8 | Undocumented. |
| `DEC IXL` | `DD 2D` | 8 | Undocumented. |
| `DEC A` | `DD 3D` | 8 | Undocumented. |
| `DEC B` | `FD 05` | 8 | Undocumented. |
| `DEC C` | `FD 0D` | 8 | Undocumented. |
| `DEC D` | `FD 15` | 8 | Undocumented. |
| `DEC E` | `FD 1D` | 8 | Undocumented. |
| `DEC IYH` | `FD 25` | 8 | Undocumented. |
| `DEC IYL` | `FD 2D` | 8 | Undocumented. |
| `DEC A` | `FD 3D` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
