---
cpu: Z80N
defined_by: Z80N
form: BSRA DE, B
mnemonic: BSRA DE, B
group: Z80N Barrel Shift
description: Shifts DE right by the number of bits in the low five bits of B, preserving the sign bit.
undocumented: false
flags:
  - flag: S
    effect: unaffected
    description: Unaffected.
  - flag: Z
    effect: unaffected
    description: Unaffected.
  - flag: Y
    effect: unaffected
    description: Unaffected.
  - flag: H
    effect: unaffected
    description: Unaffected.
  - flag: X
    effect: unaffected
    description: Unaffected.
  - flag: PV
    effect: unaffected
    description: Unaffected.
  - flag: N
    effect: unaffected
    description: Unaffected.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: ED 29
    mnemonic: BSRA DE, B
    tstates: [8]
    undocumented: false
references:
  - name: Extended Z80 instruction set, ZX Spectrum Next wiki
    url: "https://wiki.specnext.dev/Extended_Z80_instruction_set"
---

# BSRA DE, B

Shifts DE right by the number of bits in the low five bits of B, preserving the sign bit.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Unaffected. |
| H | Unaffected. |
| X | Unaffected. |
| PV | Unaffected. |
| N | Unaffected. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `BSRA DE, B` | `ED 29` | 8 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
