---
cpu: Z80
defined_by: Z80
form: SLL (IY + d), R0
mnemonic: SLL (IY + d), r
group: Rotate and Shift
description: "Shifts the byte at (IY + d) left one bit, copying bit 7 to the carry flag and setting bit 0, and stores the result in both the byte at (IY + d) and {R0}."
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
  - encoding: FD CB d 30
    mnemonic: SLL (IY + d), B
    operands: { R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 31
    mnemonic: SLL (IY + d), C
    operands: { R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 32
    mnemonic: SLL (IY + d), D
    operands: { R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 33
    mnemonic: SLL (IY + d), E
    operands: { R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 34
    mnemonic: SLL (IY + d), H
    operands: { R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 35
    mnemonic: SLL (IY + d), L
    operands: { R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 37
    mnemonic: SLL (IY + d), A
    operands: { R0: A }
    tstates: [23]
    undocumented: true
references:
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
  - name: The Undocumented Z80 Documented
    url: "http://www.z80.info/zip/z80-documented.pdf"
---

# SLL (IY + d), r

Shifts the byte at (IY + d) left one bit, copying bit 7 to the carry flag and setting bit 0, and stores the result in both the byte at (IY + d) and r.

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
| `SLL (IY + d), B` | `FD CB d 30` | 23 |  |
| `SLL (IY + d), C` | `FD CB d 31` | 23 |  |
| `SLL (IY + d), D` | `FD CB d 32` | 23 |  |
| `SLL (IY + d), E` | `FD CB d 33` | 23 |  |
| `SLL (IY + d), H` | `FD CB d 34` | 23 |  |
| `SLL (IY + d), L` | `FD CB d 35` | 23 |  |
| `SLL (IY + d), A` | `FD CB d 37` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
