---
cpu: Z80N
defined_by: Z80
form: RRCA
mnemonic: RRCA
group: Rotate and Shift
description: "Rotates A right one bit; bit 0 is copied to both bit 7 and the carry flag."
undocumented: false
flags:
  - flag: S
    effect: unaffected
    description: Unaffected.
  - flag: Z
    effect: unaffected
    description: Unaffected.
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
    effect: unaffected
    description: Unaffected.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: changed
    description: Copied from bit 0 of the original value of A.
opcodes:
  - encoding: 0F
    mnemonic: RRCA
    tstates: [4]
    undocumented: false
  - encoding: DD 0F
    mnemonic: RRCA
    tstates: [8]
    undocumented: true
  - encoding: FD 0F
    mnemonic: RRCA
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RRCA

Rotates A right one bit; bit 0 is copied to both bit 7 and the carry flag.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Copied from bit 5 of A. |
| H | Reset. |
| X | Copied from bit 3 of A. |
| PV | Unaffected. |
| N | Reset. |
| C | Copied from bit 0 of the original value of A. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `RRCA` | `0F` | 4 |  |
| `RRCA` | `DD 0F` | 8 | Undocumented. |
| `RRCA` | `FD 0F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
