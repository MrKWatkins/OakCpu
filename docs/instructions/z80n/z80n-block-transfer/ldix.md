---
cpu: Z80N
defined_by: Z80N
form: LDIX
mnemonic: LDIX
group: Z80N Block Transfer
description: Copies the byte at (HL) to (DE) unless it equals A, then increments HL and DE and decrements BC.
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
  - encoding: ED A4
    mnemonic: LDIX
    tstates: [16]
    undocumented: false
references:
  - name: Extended Z80 instruction set, ZX Spectrum Next wiki
    url: "https://wiki.specnext.dev/Extended_Z80_instruction_set"
---

# LDIX

Copies the byte at (HL) to (DE) unless it equals A, then increments HL and DE and decrements BC.

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
| `LDIX` | `ED A4` | 16 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
