---
cpu: Z80N
defined_by: Z80
form: LDD
mnemonic: LDD
group: Exchange, Block Transfer, and Search
description: Copies the byte at (HL) to (DE), then decrements HL, DE and BC.
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
    description: Set if bit 1 of the result is set, otherwise reset.
  - flag: H
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: Set if bit 3 of the result is set, otherwise reset.
  - flag: PV
    effect: changed
    description: Set if BC is not zero, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: ED A8
    mnemonic: LDD
    tstates: [16]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LDD

Copies the byte at (HL) to (DE), then decrements HL, DE and BC.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Set if bit 1 of the result is set, otherwise reset. |
| H | Reset. |
| X | Set if bit 3 of the result is set, otherwise reset. |
| PV | Set if BC is not zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `LDD` | `ED A8` | 16 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
