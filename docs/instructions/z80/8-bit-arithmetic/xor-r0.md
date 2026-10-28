---
cpu: Z80
defined_by: Z80
form: XOR R0
mnemonic: XOR r
group: 8-Bit Arithmetic
description: "Sets A to the bitwise exclusive OR of A and {R0}."
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of A.
  - flag: Z
    effect: changed
    description: Set if A is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of A.
  - flag: H
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of A.
  - flag: PV
    effect: changed
    description: Set if A has even parity, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: reset
    description: Reset.
opcodes:
  - encoding: A8
    mnemonic: XOR B
    operands: { R0: B }
    tstates: [4]
    undocumented: false
  - encoding: A9
    mnemonic: XOR C
    operands: { R0: C }
    tstates: [4]
    undocumented: false
  - encoding: AA
    mnemonic: XOR D
    operands: { R0: D }
    tstates: [4]
    undocumented: false
  - encoding: AB
    mnemonic: XOR E
    operands: { R0: E }
    tstates: [4]
    undocumented: false
  - encoding: AC
    mnemonic: XOR H
    operands: { R0: H }
    tstates: [4]
    undocumented: false
  - encoding: AD
    mnemonic: XOR L
    operands: { R0: L }
    tstates: [4]
    undocumented: false
  - encoding: AF
    mnemonic: XOR A
    operands: { R0: A }
    tstates: [4]
    undocumented: false
  - encoding: DD A8
    mnemonic: XOR B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: DD A9
    mnemonic: XOR C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: DD AA
    mnemonic: XOR D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: DD AB
    mnemonic: XOR E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: DD AC
    mnemonic: XOR IXH
    operands: { R0: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD AD
    mnemonic: XOR IXL
    operands: { R0: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD AF
    mnemonic: XOR A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
  - encoding: FD A8
    mnemonic: XOR B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: FD A9
    mnemonic: XOR C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: FD AA
    mnemonic: XOR D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: FD AB
    mnemonic: XOR E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: FD AC
    mnemonic: XOR IYH
    operands: { R0: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD AD
    mnemonic: XOR IYL
    operands: { R0: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD AF
    mnemonic: XOR A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# XOR r

Sets A to the bitwise exclusive OR of A and r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of A. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of A. |
| H | Reset. |
| X | Copied from bit 3 of A. |
| PV | Set if A has even parity, otherwise reset. |
| N | Reset. |
| C | Reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `XOR B` | `A8` | 4 |  |
| `XOR C` | `A9` | 4 |  |
| `XOR D` | `AA` | 4 |  |
| `XOR E` | `AB` | 4 |  |
| `XOR H` | `AC` | 4 |  |
| `XOR L` | `AD` | 4 |  |
| `XOR A` | `AF` | 4 |  |
| `XOR B` | `DD A8` | 8 | Undocumented. |
| `XOR C` | `DD A9` | 8 | Undocumented. |
| `XOR D` | `DD AA` | 8 | Undocumented. |
| `XOR E` | `DD AB` | 8 | Undocumented. |
| `XOR IXH` | `DD AC` | 8 | Undocumented. |
| `XOR IXL` | `DD AD` | 8 | Undocumented. |
| `XOR A` | `DD AF` | 8 | Undocumented. |
| `XOR B` | `FD A8` | 8 | Undocumented. |
| `XOR C` | `FD A9` | 8 | Undocumented. |
| `XOR D` | `FD AA` | 8 | Undocumented. |
| `XOR E` | `FD AB` | 8 | Undocumented. |
| `XOR IYH` | `FD AC` | 8 | Undocumented. |
| `XOR IYL` | `FD AD` | 8 | Undocumented. |
| `XOR A` | `FD AF` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
