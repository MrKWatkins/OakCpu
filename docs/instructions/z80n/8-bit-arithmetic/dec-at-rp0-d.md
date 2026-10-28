---
cpu: Z80N
defined_by: Z80
form: DEC (RP0 + d)
mnemonic: DEC (rr + d)
group: 8-Bit Arithmetic
description: "Decrements the byte at ({RP0} + d)."
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
  - encoding: DD 35 d
    mnemonic: DEC (IX + d)
    operands: { RP0: IX }
    tstates: [23]
    undocumented: false
  - encoding: FD 35 d
    mnemonic: DEC (IY + d)
    operands: { RP0: IY }
    tstates: [23]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# DEC (rr + d)

Decrements the byte at (rr + d).

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
| `DEC (IX + d)` | `DD 35 d` | 23 |  |
| `DEC (IY + d)` | `FD 35 d` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
