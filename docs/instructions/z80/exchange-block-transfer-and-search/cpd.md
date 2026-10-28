---
cpu: Z80
defined_by: Z80
form: CPD
mnemonic: CPD
group: Exchange, Block Transfer, and Search
description: Compares A with the byte at (HL) by subtraction, setting the flags from the result, then decrements HL and BC.
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the result.
  - flag: Z
    effect: changed
    description: Set if the result is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Set if bit 1 of (the result - the half borrow) is set, otherwise reset.
  - flag: H
    effect: changed
    description: Set if there was a borrow from bit 4, otherwise reset.
  - flag: X
    effect: changed
    description: Set if bit 3 of (the result - the half borrow) is set, otherwise reset.
  - flag: PV
    effect: changed
    description: Set if BC is not zero, otherwise reset.
  - flag: N
    effect: set
    description: Set.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: ED A9
    mnemonic: CPD
    tstates: [16]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# CPD

Compares A with the byte at (HL) by subtraction, setting the flags from the result, then decrements HL and BC.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Set if bit 1 of (the result - the half borrow) is set, otherwise reset. |
| H | Set if there was a borrow from bit 4, otherwise reset. |
| X | Set if bit 3 of (the result - the half borrow) is set, otherwise reset. |
| PV | Set if BC is not zero, otherwise reset. |
| N | Set. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `CPD` | `ED A9` | 16 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
