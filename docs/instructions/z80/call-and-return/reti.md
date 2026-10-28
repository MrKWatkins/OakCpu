---
cpu: Z80
defined_by: Z80
form: RETI
mnemonic: RETI
group: Call and Return
description: Returns from a maskable interrupt handler, popping the return address from the stack into PC and copying IFF2 to IFF1. Z80 peripherals recognise the instruction as the end of the interrupt.
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
  - encoding: ED 4D
    mnemonic: RETI
    tstates: [14]
    undocumented: false
  - encoding: ED 5D
    mnemonic: RETI
    tstates: [14]
    undocumented: true
  - encoding: ED 6D
    mnemonic: RETI
    tstates: [14]
    undocumented: true
  - encoding: ED 7D
    mnemonic: RETI
    tstates: [14]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RETI

Returns from a maskable interrupt handler, popping the return address from the stack into PC and copying IFF2 to IFF1. Z80 peripherals recognise the instruction as the end of the interrupt.

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
| `RETI` | `ED 4D` | 14 |  |
| `RETI` | `ED 5D` | 14 | Undocumented. |
| `RETI` | `ED 6D` | 14 | Undocumented. |
| `RETI` | `ED 7D` | 14 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
