---
cpu: Z80
defined_by: Z80
form: INC R0
mnemonic: INC r
group: 8-Bit Arithmetic
description: "Increments {R0}."
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
    description: "Set if ({R0} & 0x0F) equals 0x00, otherwise reset."
  - flag: X
    effect: changed
    description: "Copied from bit 3 of {R0}."
  - flag: PV
    effect: changed
    description: "Set if {R0} equals 0x80, otherwise reset."
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: 04
    mnemonic: INC B
    operands: { R0: B }
    tstates: [4]
    undocumented: false
  - encoding: 0C
    mnemonic: INC C
    operands: { R0: C }
    tstates: [4]
    undocumented: false
  - encoding: 14
    mnemonic: INC D
    operands: { R0: D }
    tstates: [4]
    undocumented: false
  - encoding: 1C
    mnemonic: INC E
    operands: { R0: E }
    tstates: [4]
    undocumented: false
  - encoding: 24
    mnemonic: INC H
    operands: { R0: H }
    tstates: [4]
    undocumented: false
  - encoding: 2C
    mnemonic: INC L
    operands: { R0: L }
    tstates: [4]
    undocumented: false
  - encoding: 3C
    mnemonic: INC A
    operands: { R0: A }
    tstates: [4]
    undocumented: false
  - encoding: DD 04
    mnemonic: INC B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 0C
    mnemonic: INC C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 14
    mnemonic: INC D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 1C
    mnemonic: INC E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 24
    mnemonic: INC IXH
    operands: { R0: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 2C
    mnemonic: INC IXL
    operands: { R0: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 3C
    mnemonic: INC A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 04
    mnemonic: INC B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 0C
    mnemonic: INC C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 14
    mnemonic: INC D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 1C
    mnemonic: INC E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 24
    mnemonic: INC IYH
    operands: { R0: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 2C
    mnemonic: INC IYL
    operands: { R0: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 3C
    mnemonic: INC A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# INC r

Increments r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of r is set, otherwise reset. |
| Z | Set if r is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Set if (r & 0x0F) equals 0x00, otherwise reset. |
| X | Copied from bit 3 of r. |
| PV | Set if r equals 0x80, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `INC B` | `04` | 4 |  |
| `INC C` | `0C` | 4 |  |
| `INC D` | `14` | 4 |  |
| `INC E` | `1C` | 4 |  |
| `INC H` | `24` | 4 |  |
| `INC L` | `2C` | 4 |  |
| `INC A` | `3C` | 4 |  |
| `INC B` | `DD 04` | 8 | Undocumented. |
| `INC C` | `DD 0C` | 8 | Undocumented. |
| `INC D` | `DD 14` | 8 | Undocumented. |
| `INC E` | `DD 1C` | 8 | Undocumented. |
| `INC IXH` | `DD 24` | 8 | Undocumented. |
| `INC IXL` | `DD 2C` | 8 | Undocumented. |
| `INC A` | `DD 3C` | 8 | Undocumented. |
| `INC B` | `FD 04` | 8 | Undocumented. |
| `INC C` | `FD 0C` | 8 | Undocumented. |
| `INC D` | `FD 14` | 8 | Undocumented. |
| `INC E` | `FD 1C` | 8 | Undocumented. |
| `INC IYH` | `FD 24` | 8 | Undocumented. |
| `INC IYL` | `FD 2C` | 8 | Undocumented. |
| `INC A` | `FD 3C` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
