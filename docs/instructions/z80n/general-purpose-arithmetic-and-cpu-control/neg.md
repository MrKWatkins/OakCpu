---
cpu: Z80N
defined_by: Z80
form: NEG
mnemonic: NEG
group: General-Purpose Arithmetic and CPU Control
description: Negates A by subtracting it from zero.
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
    effect: changed
    description: "Set if bit 4 of (the original value of A ^ A) is set, otherwise reset."
  - flag: X
    effect: changed
    description: Copied from bit 3 of A.
  - flag: PV
    effect: changed
    description: Set if A equals 0x80, otherwise reset.
  - flag: N
    effect: set
    description: Set.
  - flag: C
    effect: changed
    description: Set if A is not zero, otherwise reset.
opcodes:
  - encoding: ED 44
    mnemonic: NEG
    tstates: [8]
    undocumented: false
  - encoding: ED 4C
    mnemonic: NEG
    tstates: [8]
    undocumented: true
  - encoding: ED 54
    mnemonic: NEG
    tstates: [8]
    undocumented: true
  - encoding: ED 5C
    mnemonic: NEG
    tstates: [8]
    undocumented: true
  - encoding: ED 64
    mnemonic: NEG
    tstates: [8]
    undocumented: true
  - encoding: ED 6C
    mnemonic: NEG
    tstates: [8]
    undocumented: true
  - encoding: ED 74
    mnemonic: NEG
    tstates: [8]
    undocumented: true
  - encoding: ED 7C
    mnemonic: NEG
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# NEG

Negates A by subtracting it from zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of A. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of A. |
| H | Set if bit 4 of (the original value of A ^ A) is set, otherwise reset. |
| X | Copied from bit 3 of A. |
| PV | Set if A equals 0x80, otherwise reset. |
| N | Set. |
| C | Set if A is not zero, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `NEG` | `ED 44` | 8 |  |
| `NEG` | `ED 4C` | 8 | Undocumented. |
| `NEG` | `ED 54` | 8 | Undocumented. |
| `NEG` | `ED 5C` | 8 | Undocumented. |
| `NEG` | `ED 64` | 8 | Undocumented. |
| `NEG` | `ED 6C` | 8 | Undocumented. |
| `NEG` | `ED 74` | 8 | Undocumented. |
| `NEG` | `ED 7C` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
