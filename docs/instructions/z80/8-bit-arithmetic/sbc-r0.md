---
cpu: Z80
defined_by: Z80
form: SBC R0
mnemonic: SBC r
group: 8-Bit Arithmetic
description: "Subtracts {R0} and the carry flag from A."
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
    description: Set if there was a borrow from bit 4, otherwise reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the result.
  - flag: PV
    effect: changed
    description: Set if the subtraction overflowed, otherwise reset.
  - flag: N
    effect: set
    description: Set.
  - flag: C
    effect: changed
    description: Set if bit 8 of the result is set, otherwise reset.
opcodes:
  - encoding: 98
    mnemonic: SBC B
    operands: { R0: B }
    tstates: [4]
    undocumented: false
  - encoding: 99
    mnemonic: SBC C
    operands: { R0: C }
    tstates: [4]
    undocumented: false
  - encoding: 9A
    mnemonic: SBC D
    operands: { R0: D }
    tstates: [4]
    undocumented: false
  - encoding: 9B
    mnemonic: SBC E
    operands: { R0: E }
    tstates: [4]
    undocumented: false
  - encoding: 9C
    mnemonic: SBC H
    operands: { R0: H }
    tstates: [4]
    undocumented: false
  - encoding: 9D
    mnemonic: SBC L
    operands: { R0: L }
    tstates: [4]
    undocumented: false
  - encoding: 9F
    mnemonic: SBC A
    operands: { R0: A }
    tstates: [4]
    undocumented: false
  - encoding: DD 98
    mnemonic: SBC B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 99
    mnemonic: SBC C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 9A
    mnemonic: SBC D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 9B
    mnemonic: SBC E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 9C
    mnemonic: SBC IXH
    operands: { R0: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 9D
    mnemonic: SBC IXL
    operands: { R0: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 9F
    mnemonic: SBC A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 98
    mnemonic: SBC B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 99
    mnemonic: SBC C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 9A
    mnemonic: SBC D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 9B
    mnemonic: SBC E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 9C
    mnemonic: SBC IYH
    operands: { R0: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 9D
    mnemonic: SBC IYL
    operands: { R0: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 9F
    mnemonic: SBC A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SBC r

Subtracts r and the carry flag from A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set if there was a borrow from bit 4, otherwise reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the subtraction overflowed, otherwise reset. |
| N | Set. |
| C | Set if bit 8 of the result is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `SBC B` | `98` | 4 |  |
| `SBC C` | `99` | 4 |  |
| `SBC D` | `9A` | 4 |  |
| `SBC E` | `9B` | 4 |  |
| `SBC H` | `9C` | 4 |  |
| `SBC L` | `9D` | 4 |  |
| `SBC A` | `9F` | 4 |  |
| `SBC B` | `DD 98` | 8 | Undocumented. |
| `SBC C` | `DD 99` | 8 | Undocumented. |
| `SBC D` | `DD 9A` | 8 | Undocumented. |
| `SBC E` | `DD 9B` | 8 | Undocumented. |
| `SBC IXH` | `DD 9C` | 8 | Undocumented. |
| `SBC IXL` | `DD 9D` | 8 | Undocumented. |
| `SBC A` | `DD 9F` | 8 | Undocumented. |
| `SBC B` | `FD 98` | 8 | Undocumented. |
| `SBC C` | `FD 99` | 8 | Undocumented. |
| `SBC D` | `FD 9A` | 8 | Undocumented. |
| `SBC E` | `FD 9B` | 8 | Undocumented. |
| `SBC IYH` | `FD 9C` | 8 | Undocumented. |
| `SBC IYL` | `FD 9D` | 8 | Undocumented. |
| `SBC A` | `FD 9F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
