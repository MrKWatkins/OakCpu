---
cpu: Z80N
defined_by: Z80
form: RET C0
mnemonic: RET cc
group: Call and Return
description: "If condition {C0} is met, pops the return address from the stack into PC; otherwise continues with the next instruction."
undocumented: false
flags:
  - flag: S
    effect: unaffected
    description: Unaffected.
  - flag: Z
    effect: unaffected
    description: Unaffected.
  - flag: Y
    effect: unaffected
    description: Unaffected.
  - flag: H
    effect: unaffected
    description: Unaffected.
  - flag: X
    effect: unaffected
    description: Unaffected.
  - flag: PV
    effect: unaffected
    description: Unaffected.
  - flag: N
    effect: unaffected
    description: Unaffected.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: C0
    mnemonic: RET NZ
    operands: { C0: NZ }
    tstates: [5, 11]
    undocumented: false
  - encoding: C8
    mnemonic: RET Z
    operands: { C0: Z }
    tstates: [5, 11]
    undocumented: false
  - encoding: D0
    mnemonic: RET NC
    operands: { C0: NC }
    tstates: [5, 11]
    undocumented: false
  - encoding: D8
    mnemonic: RET C
    operands: { C0: C }
    tstates: [5, 11]
    undocumented: false
  - encoding: E0
    mnemonic: RET PO
    operands: { C0: PO }
    tstates: [5, 11]
    undocumented: false
  - encoding: E8
    mnemonic: RET PE
    operands: { C0: PE }
    tstates: [5, 11]
    undocumented: false
  - encoding: F0
    mnemonic: RET P
    operands: { C0: P }
    tstates: [5, 11]
    undocumented: false
  - encoding: F8
    mnemonic: RET M
    operands: { C0: M }
    tstates: [5, 11]
    undocumented: false
  - encoding: DD C0
    mnemonic: RET NZ
    operands: { C0: NZ }
    tstates: [9, 15]
    undocumented: true
  - encoding: DD C8
    mnemonic: RET Z
    operands: { C0: Z }
    tstates: [9, 15]
    undocumented: true
  - encoding: DD D0
    mnemonic: RET NC
    operands: { C0: NC }
    tstates: [9, 15]
    undocumented: true
  - encoding: DD D8
    mnemonic: RET C
    operands: { C0: C }
    tstates: [9, 15]
    undocumented: true
  - encoding: DD E0
    mnemonic: RET PO
    operands: { C0: PO }
    tstates: [9, 15]
    undocumented: true
  - encoding: DD E8
    mnemonic: RET PE
    operands: { C0: PE }
    tstates: [9, 15]
    undocumented: true
  - encoding: DD F0
    mnemonic: RET P
    operands: { C0: P }
    tstates: [9, 15]
    undocumented: true
  - encoding: DD F8
    mnemonic: RET M
    operands: { C0: M }
    tstates: [9, 15]
    undocumented: true
  - encoding: FD C0
    mnemonic: RET NZ
    operands: { C0: NZ }
    tstates: [9, 15]
    undocumented: true
  - encoding: FD C8
    mnemonic: RET Z
    operands: { C0: Z }
    tstates: [9, 15]
    undocumented: true
  - encoding: FD D0
    mnemonic: RET NC
    operands: { C0: NC }
    tstates: [9, 15]
    undocumented: true
  - encoding: FD D8
    mnemonic: RET C
    operands: { C0: C }
    tstates: [9, 15]
    undocumented: true
  - encoding: FD E0
    mnemonic: RET PO
    operands: { C0: PO }
    tstates: [9, 15]
    undocumented: true
  - encoding: FD E8
    mnemonic: RET PE
    operands: { C0: PE }
    tstates: [9, 15]
    undocumented: true
  - encoding: FD F0
    mnemonic: RET P
    operands: { C0: P }
    tstates: [9, 15]
    undocumented: true
  - encoding: FD F8
    mnemonic: RET M
    operands: { C0: M }
    tstates: [9, 15]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RET cc

If condition cc is met, pops the return address from the stack into PC; otherwise continues with the next instruction.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Unaffected. |
| H | Unaffected. |
| X | Unaffected. |
| PV | Unaffected. |
| N | Unaffected. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `RET NZ` | `C0` | 5 or 11 |  |
| `RET Z` | `C8` | 5 or 11 |  |
| `RET NC` | `D0` | 5 or 11 |  |
| `RET C` | `D8` | 5 or 11 |  |
| `RET PO` | `E0` | 5 or 11 |  |
| `RET PE` | `E8` | 5 or 11 |  |
| `RET P` | `F0` | 5 or 11 |  |
| `RET M` | `F8` | 5 or 11 |  |
| `RET NZ` | `DD C0` | 9 or 15 | Undocumented. |
| `RET Z` | `DD C8` | 9 or 15 | Undocumented. |
| `RET NC` | `DD D0` | 9 or 15 | Undocumented. |
| `RET C` | `DD D8` | 9 or 15 | Undocumented. |
| `RET PO` | `DD E0` | 9 or 15 | Undocumented. |
| `RET PE` | `DD E8` | 9 or 15 | Undocumented. |
| `RET P` | `DD F0` | 9 or 15 | Undocumented. |
| `RET M` | `DD F8` | 9 or 15 | Undocumented. |
| `RET NZ` | `FD C0` | 9 or 15 | Undocumented. |
| `RET Z` | `FD C8` | 9 or 15 | Undocumented. |
| `RET NC` | `FD D0` | 9 or 15 | Undocumented. |
| `RET C` | `FD D8` | 9 or 15 | Undocumented. |
| `RET PO` | `FD E0` | 9 or 15 | Undocumented. |
| `RET PE` | `FD E8` | 9 or 15 | Undocumented. |
| `RET P` | `FD F0` | 9 or 15 | Undocumented. |
| `RET M` | `FD F8` | 9 or 15 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
