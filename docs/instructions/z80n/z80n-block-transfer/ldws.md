---
cpu: Z80N
defined_by: Z80N
form: LDWS
mnemonic: LDWS
group: Z80N Block Transfer
description: Copies the byte at (HL) to (DE), then increments L and D, stepping down a column of ZX Spectrum screen memory.
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Set if bit 7 of D is set, otherwise reset.
  - flag: Z
    effect: changed
    description: Set if D is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of D.
  - flag: H
    effect: changed
    description: "Set if (D & 0x0F) equals 0x00, otherwise reset."
  - flag: X
    effect: changed
    description: Copied from bit 3 of D.
  - flag: PV
    effect: changed
    description: Set if D equals 0x80, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: ED A5
    mnemonic: LDWS
    tstates: [14]
    undocumented: false
references:
  - name: Extended Z80 instruction set, ZX Spectrum Next wiki
    url: "https://wiki.specnext.dev/Extended_Z80_instruction_set"
---

# LDWS

Copies the byte at (HL) to (DE), then increments L and D, stepping down a column of ZX Spectrum screen memory.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of D is set, otherwise reset. |
| Z | Set if D is zero, otherwise reset. |
| Y | Copied from bit 5 of D. |
| H | Set if (D & 0x0F) equals 0x00, otherwise reset. |
| X | Copied from bit 3 of D. |
| PV | Set if D equals 0x80, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `LDWS` | `ED A5` | 14 |  |

## References

- [Extended Z80 instruction set, ZX Spectrum Next wiki](https://wiki.specnext.dev/Extended_Z80_instruction_set)
