---
cpu: Z80N
defined_by: Z80
form: SRL (IY + d), R0
mnemonic: SRL (IY + d), r
group: Rotate and Shift
description: "Shifts the byte at (IY + d) right one bit, copying bit 0 to the carry flag and resetting bit 7, and stores the result in both the byte at (IY + d) and {R0}."
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
  - encoding: FD CB d 38
    mnemonic: SRL (IY + d), B
    operands: { R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 39
    mnemonic: SRL (IY + d), C
    operands: { R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 3A
    mnemonic: SRL (IY + d), D
    operands: { R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 3B
    mnemonic: SRL (IY + d), E
    operands: { R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 3C
    mnemonic: SRL (IY + d), H
    operands: { R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 3D
    mnemonic: SRL (IY + d), L
    operands: { R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 3F
    mnemonic: SRL (IY + d), A
    operands: { R0: A }
    tstates: [23]
    undocumented: true
references:
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
  - name: The Undocumented Z80 Documented
    url: "http://www.z80.info/zip/z80-documented.pdf"
---

# SRL (IY + d), r

Shifts the byte at (IY + d) right one bit, copying bit 0 to the carry flag and resetting bit 7, and stores the result in both the byte at (IY + d) and r.

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
| `SRL (IY + d), B` | `FD CB d 38` | 23 |  |
| `SRL (IY + d), C` | `FD CB d 39` | 23 |  |
| `SRL (IY + d), D` | `FD CB d 3A` | 23 |  |
| `SRL (IY + d), E` | `FD CB d 3B` | 23 |  |
| `SRL (IY + d), H` | `FD CB d 3C` | 23 |  |
| `SRL (IY + d), L` | `FD CB d 3D` | 23 |  |
| `SRL (IY + d), A` | `FD CB d 3F` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
