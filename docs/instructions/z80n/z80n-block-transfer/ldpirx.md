---
cpu: Z80N
defined_by: Z80N
form: LDPIRX
mnemonic: LDPIRX
group: Z80N Block Transfer
description: "Copies the byte at ((HL & 0xFFF8) + (E & 7)), i.e. from the eight byte aligned pattern at HL, to (DE) unless it equals A, then increments DE and decrements BC, repeating until BC is zero."
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
    description: When the instruction completes, set if bit 1 of the result is set, otherwise reset. When it repeats, set if bit 5 of PCH is set, otherwise reset.
  - flag: H
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: When the instruction completes, set if bit 3 of the result is set, otherwise reset. When it repeats, set if bit 3 of PCH is set, otherwise reset.
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
  - encoding: ED B7
    mnemonic: LDPIRX
    tstates: [16, 21]
    undocumented: false
references:
  - name: Extended Z80 instruction set, ZX Spectrum Next wiki
    url: "https://wiki.specnext.dev/Extended_Z80_instruction_set"
---

# LDPIRX

Copies the byte at ((HL & 0xFFF8) + (E & 7)), i.e. from the eight byte aligned pattern at HL, to (DE) unless it equals A, then increments DE and decrements BC, repeating until BC is zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | When the instruction completes, set if bit 1 of the result is set, otherwise reset. When it repeats, set if bit 5 of PCH is set, otherwise reset. |
| H | Reset. |
| X | When the instruction completes, set if bit 3 of the result is set, otherwise reset. When it repeats, set if bit 3 of PCH is set, otherwise reset. |
| PV | Set if BC is not zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `LDPIRX` | `ED B7` | 16 or 21 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
