---
cpu: Z80N
defined_by: Z80
form: POP RP0
mnemonic: POP rr
group: 16-Bit Load
description: "Pops {RP0} from the stack, loading its low byte from (SP) and its high byte from (SP + 1) and then adding two to SP."
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
  - encoding: C1
    mnemonic: POP BC
    operands: { RP0: BC }
    tstates: [10]
    undocumented: false
  - encoding: D1
    mnemonic: POP DE
    operands: { RP0: DE }
    tstates: [10]
    undocumented: false
  - encoding: E1
    mnemonic: POP HL
    operands: { RP0: HL }
    tstates: [10]
    undocumented: false
  - encoding: F1
    mnemonic: POP AF
    operands: { RP0: AF }
    tstates: [10]
    undocumented: false
  - encoding: DD C1
    mnemonic: POP BC
    operands: { RP0: BC }
    tstates: [14]
    undocumented: true
  - encoding: DD D1
    mnemonic: POP DE
    operands: { RP0: DE }
    tstates: [14]
    undocumented: true
  - encoding: DD E1
    mnemonic: POP IX
    operands: { RP0: IX }
    tstates: [14]
    undocumented: false
  - encoding: DD F1
    mnemonic: POP AF
    operands: { RP0: AF }
    tstates: [14]
    undocumented: true
  - encoding: FD C1
    mnemonic: POP BC
    operands: { RP0: BC }
    tstates: [14]
    undocumented: true
  - encoding: FD D1
    mnemonic: POP DE
    operands: { RP0: DE }
    tstates: [14]
    undocumented: true
  - encoding: FD E1
    mnemonic: POP IY
    operands: { RP0: IY }
    tstates: [14]
    undocumented: false
  - encoding: FD F1
    mnemonic: POP AF
    operands: { RP0: AF }
    tstates: [14]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# POP rr

Pops rr from the stack, loading its low byte from (SP) and its high byte from (SP + 1) and then adding two to SP.

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
| `POP BC` | `C1` | 10 |  |
| `POP DE` | `D1` | 10 |  |
| `POP HL` | `E1` | 10 |  |
| `POP AF` | `F1` | 10 |  |
| `POP BC` | `DD C1` | 14 | Undocumented. |
| `POP DE` | `DD D1` | 14 | Undocumented. |
| `POP IX` | `DD E1` | 14 |  |
| `POP AF` | `DD F1` | 14 | Undocumented. |
| `POP BC` | `FD C1` | 14 | Undocumented. |
| `POP DE` | `FD D1` | 14 | Undocumented. |
| `POP IY` | `FD E1` | 14 |  |
| `POP AF` | `FD F1` | 14 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
