---
cpu: Z80N
defined_by: Z80N
form: "ADD RP0, $nn"
mnemonic: "ADD rr, $nn"
group: Z80N Arithmetic
description: "Adds nn to {RP0}."
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
  - encoding: ED 34 nn
    mnemonic: "ADD HL, $nn"
    operands: { RP0: HL }
    tstates: [16]
    undocumented: false
  - encoding: ED 35 nn
    mnemonic: "ADD DE, $nn"
    operands: { RP0: DE }
    tstates: [16]
    undocumented: false
  - encoding: ED 36 nn
    mnemonic: "ADD BC, $nn"
    operands: { RP0: BC }
    tstates: [16]
    undocumented: false
references:
  - name: Extended Z80 instruction set, ZX Spectrum Next wiki
    url: "https://wiki.specnext.dev/Extended_Z80_instruction_set"
---

# ADD rr, $nn

Adds nn to rr.

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
| `ADD HL, $nn` | `ED 34 nn` | 16 |  |
| `ADD DE, $nn` | `ED 35 nn` | 16 |  |
| `ADD BC, $nn` | `ED 36 nn` | 16 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
