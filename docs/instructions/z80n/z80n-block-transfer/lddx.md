---
cpu: Z80N
defined_by: Z80N
form: LDDX
mnemonic: LDDX
group: Z80N Block Transfer
description: Copies the byte at (HL) to (DE) unless it equals A, then decrements HL, increments DE and decrements BC.
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
    description: Set if bit 1 of the result is set, otherwise reset.
  - flag: H
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: Set if bit 3 of the result is set, otherwise reset.
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
  - encoding: ED AC
    mnemonic: LDDX
    tstates: [16]
    undocumented: false
references:
  - name: Extended Z80 instruction set, ZX Spectrum Next wiki
    url: "https://wiki.specnext.dev/Extended_Z80_instruction_set"
---

# LDDX

Copies the byte at (HL) to (DE) unless it equals A, then decrements HL, increments DE and decrements BC.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Set if bit 1 of the result is set, otherwise reset. |
| H | Reset. |
| X | Set if bit 3 of the result is set, otherwise reset. |
| PV | Set if BC is not zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `LDDX` | `ED AC` | 16 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
