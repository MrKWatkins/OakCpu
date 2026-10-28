---
cpu: Z80
defined_by: Z80
form: DEC (HL)
mnemonic: DEC (HL)
group: 8-Bit Arithmetic
description: Decrements the byte at (HL).
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Set if bit 7 of the result is set, otherwise reset.
  - flag: Z
    effect: changed
    description: Set if the result is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of the result.
  - flag: H
    effect: changed
    description: "Set if (the result & 0x0F) equals 0x0F, otherwise reset."
  - flag: X
    effect: changed
    description: Copied from bit 3 of the result.
  - flag: PV
    effect: changed
    description: Set if the result equals 0x7F, otherwise reset.
  - flag: N
    effect: set
    description: Set.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: 35
    mnemonic: DEC (HL)
    tstates: [11]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# DEC (HL)

Decrements the byte at (HL).

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of the result is set, otherwise reset. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set if (the result & 0x0F) equals 0x0F, otherwise reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the result equals 0x7F, otherwise reset. |
| N | Set. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `DEC (HL)` | `35` | 11 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
