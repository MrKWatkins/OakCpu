---
cpu: Z80
defined_by: Z80
form: SUB R0
mnemonic: SUB r
group: 8-Bit Arithmetic
description: "Subtracts {R0} from A."
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
  - encoding: 90
    mnemonic: SUB B
    operands: { R0: B }
    tstates: [4]
    undocumented: false
  - encoding: 91
    mnemonic: SUB C
    operands: { R0: C }
    tstates: [4]
    undocumented: false
  - encoding: 92
    mnemonic: SUB D
    operands: { R0: D }
    tstates: [4]
    undocumented: false
  - encoding: 93
    mnemonic: SUB E
    operands: { R0: E }
    tstates: [4]
    undocumented: false
  - encoding: 94
    mnemonic: SUB H
    operands: { R0: H }
    tstates: [4]
    undocumented: false
  - encoding: 95
    mnemonic: SUB L
    operands: { R0: L }
    tstates: [4]
    undocumented: false
  - encoding: 97
    mnemonic: SUB A
    operands: { R0: A }
    tstates: [4]
    undocumented: false
  - encoding: DD 90
    mnemonic: SUB B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 91
    mnemonic: SUB C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 92
    mnemonic: SUB D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 93
    mnemonic: SUB E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 94
    mnemonic: SUB IXH
    operands: { R0: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 95
    mnemonic: SUB IXL
    operands: { R0: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 97
    mnemonic: SUB A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 90
    mnemonic: SUB B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 91
    mnemonic: SUB C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 92
    mnemonic: SUB D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 93
    mnemonic: SUB E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 94
    mnemonic: SUB IYH
    operands: { R0: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 95
    mnemonic: SUB IYL
    operands: { R0: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 97
    mnemonic: SUB A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SUB r

Subtracts r from A.

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
| `SUB B` | `90` | 4 |  |
| `SUB C` | `91` | 4 |  |
| `SUB D` | `92` | 4 |  |
| `SUB E` | `93` | 4 |  |
| `SUB H` | `94` | 4 |  |
| `SUB L` | `95` | 4 |  |
| `SUB A` | `97` | 4 |  |
| `SUB B` | `DD 90` | 8 | Undocumented. |
| `SUB C` | `DD 91` | 8 | Undocumented. |
| `SUB D` | `DD 92` | 8 | Undocumented. |
| `SUB E` | `DD 93` | 8 | Undocumented. |
| `SUB IXH` | `DD 94` | 8 | Undocumented. |
| `SUB IXL` | `DD 95` | 8 | Undocumented. |
| `SUB A` | `DD 97` | 8 | Undocumented. |
| `SUB B` | `FD 90` | 8 | Undocumented. |
| `SUB C` | `FD 91` | 8 | Undocumented. |
| `SUB D` | `FD 92` | 8 | Undocumented. |
| `SUB E` | `FD 93` | 8 | Undocumented. |
| `SUB IYH` | `FD 94` | 8 | Undocumented. |
| `SUB IYL` | `FD 95` | 8 | Undocumented. |
| `SUB A` | `FD 97` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
