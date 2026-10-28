---
cpu: Z80N
defined_by: Z80N
form: PIXELDN
mnemonic: PIXELDN
group: Z80N Miscellaneous
description: Updates HL from a ZX Spectrum screen address to the address of the pixel on the line below.
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
  - encoding: ED 93
    mnemonic: PIXELDN
    tstates: [8]
    undocumented: false
references:
  - name: Extended Z80 instruction set, ZX Spectrum Next wiki
    url: "https://wiki.specnext.dev/Extended_Z80_instruction_set"
---

# PIXELDN

Updates HL from a ZX Spectrum screen address to the address of the pixel on the line below.

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
| `PIXELDN` | `ED 93` | 8 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
