---
cpu: Z80N
defined_by: Z80
form: ADD RP0, RP1
mnemonic: ADD rr, rr'
group: 8-Bit Arithmetic
description: "Adds {RP1} to {RP0}."
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
    description: Copied from bit 5 of the high byte of the result.
  - flag: H
    effect: changed
    description: Set if there was a carry from bit 11 to bit 12, otherwise reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the high byte of the result.
  - flag: PV
    effect: unaffected
    description: Unaffected.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: changed
    description: Set if bit 16 of the result is set, otherwise reset.
opcodes:
  - encoding: 09
    mnemonic: ADD HL, BC
    operands: { RP0: HL, RP1: BC }
    tstates: [11]
    undocumented: false
  - encoding: 19
    mnemonic: ADD HL, DE
    operands: { RP0: HL, RP1: DE }
    tstates: [11]
    undocumented: false
  - encoding: 29
    mnemonic: ADD HL, HL
    operands: { RP0: HL, RP1: HL }
    tstates: [11]
    undocumented: false
  - encoding: 39
    mnemonic: ADD HL, SP
    operands: { RP0: HL, RP1: SP }
    tstates: [11]
    undocumented: false
  - encoding: DD 09
    mnemonic: ADD IX, BC
    operands: { RP0: IX, RP1: BC }
    tstates: [15]
    undocumented: false
  - encoding: DD 19
    mnemonic: ADD IX, DE
    operands: { RP0: IX, RP1: DE }
    tstates: [15]
    undocumented: false
  - encoding: DD 29
    mnemonic: ADD IX, IX
    operands: { RP0: IX, RP1: IX }
    tstates: [15]
    undocumented: false
  - encoding: DD 39
    mnemonic: ADD IX, SP
    operands: { RP0: IX, RP1: SP }
    tstates: [15]
    undocumented: false
  - encoding: FD 09
    mnemonic: ADD IY, BC
    operands: { RP0: IY, RP1: BC }
    tstates: [15]
    undocumented: false
  - encoding: FD 19
    mnemonic: ADD IY, DE
    operands: { RP0: IY, RP1: DE }
    tstates: [15]
    undocumented: false
  - encoding: FD 29
    mnemonic: ADD IY, IY
    operands: { RP0: IY, RP1: IY }
    tstates: [15]
    undocumented: false
  - encoding: FD 39
    mnemonic: ADD IY, SP
    operands: { RP0: IY, RP1: SP }
    tstates: [15]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# ADD rr, rr'

Adds rr' to rr.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Copied from bit 5 of the high byte of the result. |
| H | Set if there was a carry from bit 11 to bit 12, otherwise reset. |
| X | Copied from bit 3 of the high byte of the result. |
| PV | Unaffected. |
| N | Reset. |
| C | Set if bit 16 of the result is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `ADD HL, BC` | `09` | 11 |  |
| `ADD HL, DE` | `19` | 11 |  |
| `ADD HL, HL` | `29` | 11 |  |
| `ADD HL, SP` | `39` | 11 |  |
| `ADD IX, BC` | `DD 09` | 15 |  |
| `ADD IX, DE` | `DD 19` | 15 |  |
| `ADD IX, IX` | `DD 29` | 15 |  |
| `ADD IX, SP` | `DD 39` | 15 |  |
| `ADD IY, BC` | `FD 09` | 15 |  |
| `ADD IY, DE` | `FD 19` | 15 |  |
| `ADD IY, IY` | `FD 29` | 15 |  |
| `ADD IY, SP` | `FD 39` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
