---
cpu: Z80
defined_by: Z80
form: LDIR
mnemonic: LDIR
group: Exchange, Block Transfer, and Search
description: Copies the byte at (HL) to (DE), then increments HL and DE and decrements BC, repeating until BC is zero.
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
  - encoding: ED B0
    mnemonic: LDIR
    tstates: [16, 21]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LDIR

Copies the byte at (HL) to (DE), then increments HL and DE and decrements BC, repeating until BC is zero.

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
| `LDIR` | `ED B0` | 16 or 21 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
