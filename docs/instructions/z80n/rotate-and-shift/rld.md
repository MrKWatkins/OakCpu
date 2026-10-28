---
cpu: Z80N
defined_by: Z80
form: RLD
mnemonic: RLD
group: Rotate and Shift
description: "Rotates the low nibble of A and the two nibbles of the byte at (HL) left one nibble as a twelve bit value: the low nibble of A moves to the low nibble of (HL), the low nibble of (HL) to its high nibble, and the high nibble of (HL) to the low nibble of A."
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
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of A.
  - flag: PV
    effect: changed
    description: Set if A has even parity, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: ED 6F
    mnemonic: RLD
    tstates: [18]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RLD

Rotates the low nibble of A and the two nibbles of the byte at (HL) left one nibble as a twelve bit value: the low nibble of A moves to the low nibble of (HL), the low nibble of (HL) to its high nibble, and the high nibble of (HL) to the low nibble of A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of A. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of A. |
| H | Reset. |
| X | Copied from bit 3 of A. |
| PV | Set if A has even parity, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `RLD` | `ED 6F` | 18 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
