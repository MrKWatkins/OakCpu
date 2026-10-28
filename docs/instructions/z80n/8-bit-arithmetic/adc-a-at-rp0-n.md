---
cpu: Z80N
defined_by: Z80
form: ADC A, (RP0 + n)
mnemonic: ADC A, (rr + n)
group: 8-Bit Arithmetic
description: "Adds the byte at ({RP0} + n) and the carry flag to A."
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the result.
  - flag: Z
    effect: changed
    description: Set if A is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of the result.
  - flag: H
    effect: changed
    description: Set if there was a carry from bit 3 to bit 4, otherwise reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the result.
  - flag: PV
    effect: changed
    description: Set if the addition overflowed, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: changed
    description: Set if bit 8 of the result is set, otherwise reset.
opcodes:
  - encoding: DD 8E n
    mnemonic: ADC A, (IX + n)
    operands: { RP0: IX }
    tstates: [19]
    undocumented: false
  - encoding: FD 8E n
    mnemonic: ADC A, (IY + n)
    operands: { RP0: IY }
    tstates: [19]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# ADC A, (rr + n)

Adds the byte at (rr + n) and the carry flag to A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set if there was a carry from bit 3 to bit 4, otherwise reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the addition overflowed, otherwise reset. |
| N | Reset. |
| C | Set if bit 8 of the result is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `ADC A, (IX + n)` | `DD 8E n` | 19 |  |
| `ADC A, (IY + n)` | `FD 8E n` | 19 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
