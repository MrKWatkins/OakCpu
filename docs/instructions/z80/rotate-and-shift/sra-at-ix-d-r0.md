---
cpu: Z80
defined_by: Z80
form: SRA (IX + d), R0
mnemonic: SRA (IX + d), r
group: Rotate and Shift
description: "Shifts the byte at (IX + d) right one bit, copying bit 0 to the carry flag and keeping bit 7 to preserve the sign, and stores the result in both the byte at (IX + d) and {R0}."
undocumented: true
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the result.
  - flag: Z
    effect: changed
    description: Set if the result is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of the result.
  - flag: H
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the result.
  - flag: PV
    effect: changed
    description: Set if the result has even parity, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: changed
    description: Copied from bit 0 of the value read.
opcodes:
  - encoding: DD CB d 28
    mnemonic: SRA (IX + d), B
    operands: { R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 29
    mnemonic: SRA (IX + d), C
    operands: { R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 2A
    mnemonic: SRA (IX + d), D
    operands: { R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 2B
    mnemonic: SRA (IX + d), E
    operands: { R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 2C
    mnemonic: SRA (IX + d), H
    operands: { R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 2D
    mnemonic: SRA (IX + d), L
    operands: { R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 2F
    mnemonic: SRA (IX + d), A
    operands: { R0: A }
    tstates: [23]
    undocumented: true
references:
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
  - name: The Undocumented Z80 Documented
    url: "http://www.z80.info/zip/z80-documented.pdf"
---

# SRA (IX + d), r

Shifts the byte at (IX + d) right one bit, copying bit 0 to the carry flag and keeping bit 7 to preserve the sign, and stores the result in both the byte at (IX + d) and r.

This instruction is undocumented.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the result has even parity, otherwise reset. |
| N | Reset. |
| C | Copied from bit 0 of the value read. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `SRA (IX + d), B` | `DD CB d 28` | 23 |  |
| `SRA (IX + d), C` | `DD CB d 29` | 23 |  |
| `SRA (IX + d), D` | `DD CB d 2A` | 23 |  |
| `SRA (IX + d), E` | `DD CB d 2B` | 23 |  |
| `SRA (IX + d), H` | `DD CB d 2C` | 23 |  |
| `SRA (IX + d), L` | `DD CB d 2D` | 23 |  |
| `SRA (IX + d), A` | `DD CB d 2F` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
