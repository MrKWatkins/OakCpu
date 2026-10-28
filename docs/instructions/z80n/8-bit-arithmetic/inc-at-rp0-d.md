---
cpu: Z80N
defined_by: Z80
form: INC (RP0 + d)
mnemonic: INC (rr + d)
group: 8-Bit Arithmetic
description: "Increments the byte at ({RP0} + d)."
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
    description: "Set if (the result & 0x0F) equals 0x00, otherwise reset."
  - flag: X
    effect: changed
    description: Copied from bit 3 of the result.
  - flag: PV
    effect: changed
    description: Set if the result equals 0x80, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: DD 34 d
    mnemonic: INC (IX + d)
    operands: { RP0: IX }
    tstates: [23]
    undocumented: false
  - encoding: FD 34 d
    mnemonic: INC (IY + d)
    operands: { RP0: IY }
    tstates: [23]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# INC (rr + d)

Increments the byte at (rr + d).

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of the result is set, otherwise reset. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set if (the result & 0x0F) equals 0x00, otherwise reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the result equals 0x80, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `INC (IX + d)` | `DD 34 d` | 23 |  |
| `INC (IY + d)` | `FD 34 d` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
