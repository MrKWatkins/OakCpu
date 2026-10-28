---
cpu: Z80
defined_by: Z80
form: PUSH RP0
mnemonic: PUSH rr
group: 16-Bit Load
description: "Pushes {RP0} onto the stack, subtracting two from SP and then storing its high byte at (SP + 1) and its low byte at (SP)."
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
  - encoding: C5
    mnemonic: PUSH BC
    operands: { RP0: BC }
    tstates: [11]
    undocumented: false
  - encoding: D5
    mnemonic: PUSH DE
    operands: { RP0: DE }
    tstates: [11]
    undocumented: false
  - encoding: E5
    mnemonic: PUSH HL
    operands: { RP0: HL }
    tstates: [11]
    undocumented: false
  - encoding: F5
    mnemonic: PUSH AF
    operands: { RP0: AF }
    tstates: [11]
    undocumented: false
  - encoding: DD C5
    mnemonic: PUSH BC
    operands: { RP0: BC }
    tstates: [15]
    undocumented: true
  - encoding: DD D5
    mnemonic: PUSH DE
    operands: { RP0: DE }
    tstates: [15]
    undocumented: true
  - encoding: DD E5
    mnemonic: PUSH IX
    operands: { RP0: IX }
    tstates: [15]
    undocumented: false
  - encoding: DD F5
    mnemonic: PUSH AF
    operands: { RP0: AF }
    tstates: [15]
    undocumented: true
  - encoding: FD C5
    mnemonic: PUSH BC
    operands: { RP0: BC }
    tstates: [15]
    undocumented: true
  - encoding: FD D5
    mnemonic: PUSH DE
    operands: { RP0: DE }
    tstates: [15]
    undocumented: true
  - encoding: FD E5
    mnemonic: PUSH IY
    operands: { RP0: IY }
    tstates: [15]
    undocumented: false
  - encoding: FD F5
    mnemonic: PUSH AF
    operands: { RP0: AF }
    tstates: [15]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# PUSH rr

Pushes rr onto the stack, subtracting two from SP and then storing its high byte at (SP + 1) and its low byte at (SP).

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
| `PUSH BC` | `C5` | 11 |  |
| `PUSH DE` | `D5` | 11 |  |
| `PUSH HL` | `E5` | 11 |  |
| `PUSH AF` | `F5` | 11 |  |
| `PUSH BC` | `DD C5` | 15 | Undocumented. |
| `PUSH DE` | `DD D5` | 15 | Undocumented. |
| `PUSH IX` | `DD E5` | 15 |  |
| `PUSH AF` | `DD F5` | 15 | Undocumented. |
| `PUSH BC` | `FD C5` | 15 | Undocumented. |
| `PUSH DE` | `FD D5` | 15 | Undocumented. |
| `PUSH IY` | `FD E5` | 15 |  |
| `PUSH AF` | `FD F5` | 15 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
