---
cpu: Z80N
defined_by: Z80
form: LD RP0, RP1
mnemonic: LD rr, rr'
group: 16-Bit Load
description: "Copies {RP1} into {RP0}."
undocumented: false
flags:
  - flag: S
    effect: unaffected
    description: Unaffected.
  - flag: Z
    effect: unaffected
    description: Unaffected.
  - flag: Y
    effect: unaffected
    description: Unaffected.
  - flag: H
    effect: unaffected
    description: Unaffected.
  - flag: X
    effect: unaffected
    description: Unaffected.
  - flag: PV
    effect: unaffected
    description: Unaffected.
  - flag: N
    effect: unaffected
    description: Unaffected.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: F9
    mnemonic: LD SP, HL
    operands: { RP0: SP, RP1: HL }
    tstates: [6]
    undocumented: false
  - encoding: DD F9
    mnemonic: LD SP, IX
    operands: { RP0: SP, RP1: IX }
    tstates: [10]
    undocumented: false
  - encoding: FD F9
    mnemonic: LD SP, IY
    operands: { RP0: SP, RP1: IY }
    tstates: [10]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD rr, rr'

Copies rr' into rr.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Unaffected. |
| Z | Unaffected. |
| Y | Unaffected. |
| H | Unaffected. |
| X | Unaffected. |
| PV | Unaffected. |
| N | Unaffected. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `LD SP, HL` | `F9` | 6 |  |
| `LD SP, IX` | `DD F9` | 10 |  |
| `LD SP, IY` | `FD F9` | 10 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
