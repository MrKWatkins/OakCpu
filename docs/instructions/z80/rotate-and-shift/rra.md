---
cpu: Z80
defined_by: Z80
form: RRA
mnemonic: RRA
group: Rotate and Shift
description: "Rotates A right one bit through the carry flag; the carry flag is copied to bit 7 and bit 0 to the carry flag."
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
    description: Set if bit 0 of the original value of A is set, otherwise reset.
opcodes:
  - encoding: 1F
    mnemonic: RRA
    tstates: [4]
    undocumented: false
  - encoding: DD 1F
    mnemonic: RRA
    tstates: [8]
    undocumented: true
  - encoding: FD 1F
    mnemonic: RRA
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RRA

Rotates A right one bit through the carry flag; the carry flag is copied to bit 7 and bit 0 to the carry flag.

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
| C | Set if bit 0 of the original value of A is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `RRA` | `1F` | 4 |  |
| `RRA` | `DD 1F` | 8 | Undocumented. |
| `RRA` | `FD 1F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
