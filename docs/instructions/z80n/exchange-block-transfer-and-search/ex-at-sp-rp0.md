---
cpu: Z80N
defined_by: Z80
form: EX (SP), RP0
mnemonic: EX (SP), rr
group: Exchange, Block Transfer, and Search
description: "Exchanges {RP0} with the word at the top of the stack, its low byte with (SP) and its high byte with (SP + 1)."
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
  - encoding: E3
    mnemonic: EX (SP), HL
    operands: { RP0: HL }
    tstates: [19]
    undocumented: false
  - encoding: DD E3
    mnemonic: EX (SP), IX
    operands: { RP0: IX }
    tstates: [23]
    undocumented: false
  - encoding: FD E3
    mnemonic: EX (SP), IY
    operands: { RP0: IY }
    tstates: [23]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# EX (SP), rr

Exchanges rr with the word at the top of the stack, its low byte with (SP) and its high byte with (SP + 1).

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
| `EX (SP), HL` | `E3` | 19 |  |
| `EX (SP), IX` | `DD E3` | 23 |  |
| `EX (SP), IY` | `FD E3` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
