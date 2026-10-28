---
cpu: Z80N
defined_by: Z80
form: RETN
mnemonic: RETN
group: Call and Return
description: Returns from a non-maskable interrupt handler, popping the return address from the stack into PC and copying IFF2 to IFF1 to restore the interrupt state from before the interrupt.
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
  - encoding: ED 45
    mnemonic: RETN
    tstates: [14]
    undocumented: false
  - encoding: ED 55
    mnemonic: RETN
    tstates: [14]
    undocumented: true
  - encoding: ED 65
    mnemonic: RETN
    tstates: [14]
    undocumented: true
  - encoding: ED 75
    mnemonic: RETN
    tstates: [14]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RETN

Returns from a non-maskable interrupt handler, popping the return address from the stack into PC and copying IFF2 to IFF1 to restore the interrupt state from before the interrupt.

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
| `RETN` | `ED 45` | 14 |  |
| `RETN` | `ED 55` | 14 | Undocumented. |
| `RETN` | `ED 65` | 14 | Undocumented. |
| `RETN` | `ED 75` | 14 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
