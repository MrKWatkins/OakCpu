---
cpu: Z80
defined_by: Z80
form: ADC HL, RP0
mnemonic: ADC HL, rr
group: 16-Bit Arithmetic
description: "Adds {RP0} and the carry flag to HL."
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
    description: Set if there was a carry from bit 11 to bit 12, otherwise reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the high byte of the result.
  - flag: PV
    effect: changed
    description: Set if the addition overflowed, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: changed
    description: Set if bit 16 of the result is set, otherwise reset.
opcodes:
  - encoding: ED 4A
    mnemonic: ADC HL, BC
    operands: { RP0: BC }
    tstates: [15]
    undocumented: false
  - encoding: ED 5A
    mnemonic: ADC HL, DE
    operands: { RP0: DE }
    tstates: [15]
    undocumented: false
  - encoding: ED 6A
    mnemonic: ADC HL, HL
    operands: { RP0: HL }
    tstates: [15]
    undocumented: false
  - encoding: ED 7A
    mnemonic: ADC HL, SP
    operands: { RP0: SP }
    tstates: [15]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# ADC HL, rr

Adds rr and the carry flag to HL.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the high byte of the result. |
| Z | Set if the 16-bit result is zero, otherwise reset. |
| Y | Copied from bit 5 of the high byte of the result. |
| H | Set if there was a carry from bit 11 to bit 12, otherwise reset. |
| X | Copied from bit 3 of the high byte of the result. |
| PV | Set if the addition overflowed, otherwise reset. |
| N | Reset. |
| C | Set if bit 16 of the result is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `ADC HL, BC` | `ED 4A` | 15 |  |
| `ADC HL, DE` | `ED 5A` | 15 |  |
| `ADC HL, HL` | `ED 6A` | 15 |  |
| `ADC HL, SP` | `ED 7A` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
