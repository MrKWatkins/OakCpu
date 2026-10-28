---
cpu: Z80N
defined_by: Z80
form: RLC (IY + d), R0
mnemonic: RLC (IY + d), r
group: Rotate and Shift
description: "Rotates the byte at (IY + d) left one bit, copying bit 7 to both bit 0 and the carry flag, and stores the result in both the byte at (IY + d) and {R0}."
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
  - encoding: FD CB d 00
    mnemonic: RLC (IY + d), B
    operands: { R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 01
    mnemonic: RLC (IY + d), C
    operands: { R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 02
    mnemonic: RLC (IY + d), D
    operands: { R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 03
    mnemonic: RLC (IY + d), E
    operands: { R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 04
    mnemonic: RLC (IY + d), H
    operands: { R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 05
    mnemonic: RLC (IY + d), L
    operands: { R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d 07
    mnemonic: RLC (IY + d), A
    operands: { R0: A }
    tstates: [23]
    undocumented: true
references:
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
  - name: The Undocumented Z80 Documented
    url: "http://www.z80.info/zip/z80-documented.pdf"
---

# RLC (IY + d), r

Rotates the byte at (IY + d) left one bit, copying bit 7 to both bit 0 and the carry flag, and stores the result in both the byte at (IY + d) and r.

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
| `RLC (IY + d), B` | `FD CB d 00` | 23 |  |
| `RLC (IY + d), C` | `FD CB d 01` | 23 |  |
| `RLC (IY + d), D` | `FD CB d 02` | 23 |  |
| `RLC (IY + d), E` | `FD CB d 03` | 23 |  |
| `RLC (IY + d), H` | `FD CB d 04` | 23 |  |
| `RLC (IY + d), L` | `FD CB d 05` | 23 |  |
| `RLC (IY + d), A` | `FD CB d 07` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
