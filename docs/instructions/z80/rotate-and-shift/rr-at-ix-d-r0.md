---
cpu: Z80
defined_by: Z80
form: RR (IX + d), R0
mnemonic: RR (IX + d), r
group: Rotate and Shift
description: "Rotates the byte at (IX + d) right one bit through the carry flag, copying the carry flag to bit 7 and bit 0 to the carry flag, and stores the result in both the byte at (IX + d) and {R0}."
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
  - encoding: DD CB d 18
    mnemonic: RR (IX + d), B
    operands: { R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 19
    mnemonic: RR (IX + d), C
    operands: { R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 1A
    mnemonic: RR (IX + d), D
    operands: { R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 1B
    mnemonic: RR (IX + d), E
    operands: { R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 1C
    mnemonic: RR (IX + d), H
    operands: { R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 1D
    mnemonic: RR (IX + d), L
    operands: { R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 1F
    mnemonic: RR (IX + d), A
    operands: { R0: A }
    tstates: [23]
    undocumented: true
references:
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
  - name: The Undocumented Z80 Documented
    url: "http://www.z80.info/zip/z80-documented.pdf"
---

# RR (IX + d), r

Rotates the byte at (IX + d) right one bit through the carry flag, copying the carry flag to bit 7 and bit 0 to the carry flag, and stores the result in both the byte at (IX + d) and r.

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
| `RR (IX + d), B` | `DD CB d 18` | 23 |  |
| `RR (IX + d), C` | `DD CB d 19` | 23 |  |
| `RR (IX + d), D` | `DD CB d 1A` | 23 |  |
| `RR (IX + d), E` | `DD CB d 1B` | 23 |  |
| `RR (IX + d), H` | `DD CB d 1C` | 23 |  |
| `RR (IX + d), L` | `DD CB d 1D` | 23 |  |
| `RR (IX + d), A` | `DD CB d 1F` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
