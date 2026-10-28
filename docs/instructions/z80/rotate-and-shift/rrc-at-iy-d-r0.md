---
cpu: Z80
defined_by: Z80
form: RRC (IY + d), R0
mnemonic: RRC (IY + d), r
group: Rotate and Shift
description: "Rotates the byte at (IY + d) right one bit, copying bit 0 to both bit 7 and the carry flag, and stores the result in both the byte at (IY + d) and {R0}."
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
  - encoding: FD CB d 08
    mnemonic: RRC (IY + d), B
    operands: { R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 09
    mnemonic: RRC (IY + d), C
    operands: { R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 0A
    mnemonic: RRC (IY + d), D
    operands: { R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 0B
    mnemonic: RRC (IY + d), E
    operands: { R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 0C
    mnemonic: RRC (IY + d), H
    operands: { R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 0D
    mnemonic: RRC (IY + d), L
    operands: { R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 0F
    mnemonic: RRC (IY + d), A
    operands: { R0: A }
    tstates: [23]
    undocumented: true
references:
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
  - name: The Undocumented Z80 Documented
    url: "http://www.z80.info/zip/z80-documented.pdf"
---

# RRC (IY + d), r

Rotates the byte at (IY + d) right one bit, copying bit 0 to both bit 7 and the carry flag, and stores the result in both the byte at (IY + d) and r.

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
| `RRC (IY + d), B` | `FD CB d 08` | 23 |  |
| `RRC (IY + d), C` | `FD CB d 09` | 23 |  |
| `RRC (IY + d), D` | `FD CB d 0A` | 23 |  |
| `RRC (IY + d), E` | `FD CB d 0B` | 23 |  |
| `RRC (IY + d), H` | `FD CB d 0C` | 23 |  |
| `RRC (IY + d), L` | `FD CB d 0D` | 23 |  |
| `RRC (IY + d), A` | `FD CB d 0F` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
