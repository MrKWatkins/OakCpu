---
cpu: Z80N
defined_by: Z80N
form: ADD RP0, A
mnemonic: ADD rr, A
group: Z80N Arithmetic
description: "Adds A to {RP0}."
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
    effect: reset
    description: Reset.
opcodes:
  - encoding: ED 31
    mnemonic: ADD HL, A
    operands: { RP0: HL }
    tstates: [8]
    undocumented: false
  - encoding: ED 32
    mnemonic: ADD DE, A
    operands: { RP0: DE }
    tstates: [8]
    undocumented: false
  - encoding: ED 33
    mnemonic: ADD BC, A
    operands: { RP0: BC }
    tstates: [8]
    undocumented: false
references:
  - name: Extended Z80 instruction set, ZX Spectrum Next wiki
    url: "https://wiki.specnext.dev/Extended_Z80_instruction_set"
---

# ADD rr, A

Adds A to rr.

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
| C | Reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `ADD HL, A` | `ED 31` | 8 |  |
| `ADD DE, A` | `ED 32` | 8 |  |
| `ADD BC, A` | `ED 33` | 8 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
