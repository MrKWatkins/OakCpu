---
cpu: Z80N
defined_by: Z80
form: ADC A, R0
mnemonic: ADC A, r
group: 8-Bit Arithmetic
description: "Adds {R0} and the carry flag to A."
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the result.
  - flag: Z
    effect: changed
    description: Set if A is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of the result.
  - flag: H
    effect: changed
    description: Set if there was a carry from bit 3 to bit 4, otherwise reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the result.
  - flag: PV
    effect: changed
    description: Set if the addition overflowed, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: changed
    description: Set if bit 8 of the result is set, otherwise reset.
opcodes:
  - encoding: 88
    mnemonic: ADC A, B
    operands: { R0: B }
    tstates: [4]
    undocumented: false
  - encoding: 89
    mnemonic: ADC A, C
    operands: { R0: C }
    tstates: [4]
    undocumented: false
  - encoding: 8A
    mnemonic: ADC A, D
    operands: { R0: D }
    tstates: [4]
    undocumented: false
  - encoding: 8B
    mnemonic: ADC A, E
    operands: { R0: E }
    tstates: [4]
    undocumented: false
  - encoding: 8C
    mnemonic: ADC A, H
    operands: { R0: H }
    tstates: [4]
    undocumented: false
  - encoding: 8D
    mnemonic: ADC A, L
    operands: { R0: L }
    tstates: [4]
    undocumented: false
  - encoding: 8F
    mnemonic: ADC A, A
    operands: { R0: A }
    tstates: [4]
    undocumented: false
  - encoding: DD 88
    mnemonic: ADC A, B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 89
    mnemonic: ADC A, C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 8A
    mnemonic: ADC A, D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 8B
    mnemonic: ADC A, E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 8C
    mnemonic: ADC A, IXH
    operands: { R0: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 8D
    mnemonic: ADC A, IXL
    operands: { R0: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 8F
    mnemonic: ADC A, A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 88
    mnemonic: ADC A, B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 89
    mnemonic: ADC A, C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 8A
    mnemonic: ADC A, D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 8B
    mnemonic: ADC A, E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 8C
    mnemonic: ADC A, IYH
    operands: { R0: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 8D
    mnemonic: ADC A, IYL
    operands: { R0: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 8F
    mnemonic: ADC A, A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# ADC A, r

Adds r and the carry flag to A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set if there was a carry from bit 3 to bit 4, otherwise reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the addition overflowed, otherwise reset. |
| N | Reset. |
| C | Set if bit 8 of the result is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `ADC A, B` | `88` | 4 |  |
| `ADC A, C` | `89` | 4 |  |
| `ADC A, D` | `8A` | 4 |  |
| `ADC A, E` | `8B` | 4 |  |
| `ADC A, H` | `8C` | 4 |  |
| `ADC A, L` | `8D` | 4 |  |
| `ADC A, A` | `8F` | 4 |  |
| `ADC A, B` | `DD 88` | 8 | Undocumented. |
| `ADC A, C` | `DD 89` | 8 | Undocumented. |
| `ADC A, D` | `DD 8A` | 8 | Undocumented. |
| `ADC A, E` | `DD 8B` | 8 | Undocumented. |
| `ADC A, IXH` | `DD 8C` | 8 | Undocumented. |
| `ADC A, IXL` | `DD 8D` | 8 | Undocumented. |
| `ADC A, A` | `DD 8F` | 8 | Undocumented. |
| `ADC A, B` | `FD 88` | 8 | Undocumented. |
| `ADC A, C` | `FD 89` | 8 | Undocumented. |
| `ADC A, D` | `FD 8A` | 8 | Undocumented. |
| `ADC A, E` | `FD 8B` | 8 | Undocumented. |
| `ADC A, IYH` | `FD 8C` | 8 | Undocumented. |
| `ADC A, IYL` | `FD 8D` | 8 | Undocumented. |
| `ADC A, A` | `FD 8F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
