---
cpu: Z80N
defined_by: Z80
form: RLCA
mnemonic: RLCA
group: Rotate and Shift
description: "Rotates A left one bit; bit 7 is copied to both bit 0 and the carry flag."
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
    description: Set if bit 7 of the original value of A is set, otherwise reset.
opcodes:
  - encoding: 07
    mnemonic: RLCA
    tstates: [4]
    undocumented: false
  - encoding: DD 07
    mnemonic: RLCA
    tstates: [8]
    undocumented: true
  - encoding: FD 07
    mnemonic: RLCA
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RLCA

Rotates A left one bit; bit 7 is copied to both bit 0 and the carry flag.

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
| C | Set if bit 7 of the original value of A is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `RLCA` | `07` | 4 |  |
| `RLCA` | `DD 07` | 8 | Undocumented. |
| `RLCA` | `FD 07` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
