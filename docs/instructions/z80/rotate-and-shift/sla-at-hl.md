---
cpu: Z80
defined_by: Z80
form: SLA (HL)
mnemonic: SLA (HL)
group: Rotate and Shift
description: "Shifts the byte at (HL) left one bit; bit 7 is copied to the carry flag and bit 0 is reset."
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the result.
  - flag: Z
    effect: changed
    description: Set if the result is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of the result.
  - flag: H
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the result.
  - flag: PV
    effect: changed
    description: Set if the result has even parity, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: changed
    description: Set if bit 7 of the value read is set, otherwise reset.
opcodes:
  - encoding: CB 26
    mnemonic: SLA (HL)
    tstates: [15]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SLA (HL)

Shifts the byte at (HL) left one bit; bit 7 is copied to the carry flag and bit 0 is reset.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the result has even parity, otherwise reset. |
| N | Reset. |
| C | Set if bit 7 of the value read is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `SLA (HL)` | `CB 26` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
