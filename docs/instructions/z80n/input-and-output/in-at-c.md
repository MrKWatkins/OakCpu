---
cpu: Z80N
defined_by: Z80
form: IN (C)
mnemonic: IN (C)
group: Input and Output
description: Reads a byte from the port addressed by BC, setting the flags from it and discarding the value.
undocumented: true
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the value read.
  - flag: Z
    effect: changed
    description: Set if the value read is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of the value read.
  - flag: H
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the value read.
  - flag: PV
    effect: changed
    description: Set if the value read has even parity, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: ED 70
    mnemonic: IN (C)
    tstates: [12]
    undocumented: true
references:
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
  - name: The Undocumented Z80 Documented
    url: "http://www.z80.info/zip/z80-documented.pdf"
---

# IN (C)

Reads a byte from the port addressed by BC, setting the flags from it and discarding the value.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the value read. |
| Z | Set if the value read is zero, otherwise reset. |
| Y | Copied from bit 5 of the value read. |
| H | Reset. |
| X | Copied from bit 3 of the value read. |
| PV | Set if the value read has even parity, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `IN (C)` | `ED 70` | 12 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
