---
cpu: Z80
defined_by: Z80
form: SLL (IX + d), R0
mnemonic: SLL (IX + d), r
group: Rotate and Shift
description: "Shifts the byte at (IX + d) left one bit, copying bit 7 to the carry flag and setting bit 0, and stores the result in both the byte at (IX + d) and {R0}."
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
    description: Set if bit 7 of the value read is set, otherwise reset.
opcodes:
  - encoding: DD CB d 30
    mnemonic: SLL (IX + d), B
    operands: { R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 31
    mnemonic: SLL (IX + d), C
    operands: { R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 32
    mnemonic: SLL (IX + d), D
    operands: { R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 33
    mnemonic: SLL (IX + d), E
    operands: { R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 34
    mnemonic: SLL (IX + d), H
    operands: { R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 35
    mnemonic: SLL (IX + d), L
    operands: { R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 37
    mnemonic: SLL (IX + d), A
    operands: { R0: A }
    tstates: [23]
    undocumented: true
references:
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
  - name: The Undocumented Z80 Documented
    url: "http://www.z80.info/zip/z80-documented.pdf"
---

# SLL (IX + d), r

Shifts the byte at (IX + d) left one bit, copying bit 7 to the carry flag and setting bit 0, and stores the result in both the byte at (IX + d) and r.

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
| C | Set if bit 7 of the value read is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `SLL (IX + d), B` | `DD CB d 30` | 23 |  |
| `SLL (IX + d), C` | `DD CB d 31` | 23 |  |
| `SLL (IX + d), D` | `DD CB d 32` | 23 |  |
| `SLL (IX + d), E` | `DD CB d 33` | 23 |  |
| `SLL (IX + d), H` | `DD CB d 34` | 23 |  |
| `SLL (IX + d), L` | `DD CB d 35` | 23 |  |
| `SLL (IX + d), A` | `DD CB d 37` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
