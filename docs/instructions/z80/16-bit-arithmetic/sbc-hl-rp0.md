---
cpu: Z80
defined_by: Z80
form: SBC HL, RP0
mnemonic: SBC HL, rr
group: 16-Bit Arithmetic
description: "Subtracts {RP0} and the carry flag from HL."
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the high byte of the result.
  - flag: Z
    effect: changed
    description: Set if the 16-bit result is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of the high byte of the result.
  - flag: H
    effect: changed
    description: Set if there was a borrow from bit 12, otherwise reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the high byte of the result.
  - flag: PV
    effect: changed
    description: Set if the subtraction overflowed, otherwise reset.
  - flag: N
    effect: set
    description: Set.
  - flag: C
    effect: changed
    description: Set if bit 16 of the result is set, otherwise reset.
opcodes:
  - encoding: ED 42
    mnemonic: SBC HL, BC
    operands: { RP0: BC }
    tstates: [15]
    undocumented: false
  - encoding: ED 52
    mnemonic: SBC HL, DE
    operands: { RP0: DE }
    tstates: [15]
    undocumented: false
  - encoding: ED 62
    mnemonic: SBC HL, HL
    operands: { RP0: HL }
    tstates: [15]
    undocumented: false
  - encoding: ED 72
    mnemonic: SBC HL, SP
    operands: { RP0: SP }
    tstates: [15]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SBC HL, rr

Subtracts rr and the carry flag from HL.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the high byte of the result. |
| Z | Set if the 16-bit result is zero, otherwise reset. |
| Y | Copied from bit 5 of the high byte of the result. |
| H | Set if there was a borrow from bit 12, otherwise reset. |
| X | Copied from bit 3 of the high byte of the result. |
| PV | Set if the subtraction overflowed, otherwise reset. |
| N | Set. |
| C | Set if bit 16 of the result is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `SBC HL, BC` | `ED 42` | 15 |  |
| `SBC HL, DE` | `ED 52` | 15 |  |
| `SBC HL, HL` | `ED 62` | 15 |  |
| `SBC HL, SP` | `ED 72` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
