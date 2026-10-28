---
cpu: Z80N
defined_by: Z80
form: DEC RP0
mnemonic: DEC rr
group: 16-Bit Arithmetic
description: "Decrements {RP0}."
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
  - encoding: 0B
    mnemonic: DEC BC
    operands: { RP0: BC }
    tstates: [6]
    undocumented: false
  - encoding: 1B
    mnemonic: DEC DE
    operands: { RP0: DE }
    tstates: [6]
    undocumented: false
  - encoding: 2B
    mnemonic: DEC HL
    operands: { RP0: HL }
    tstates: [6]
    undocumented: false
  - encoding: 3B
    mnemonic: DEC SP
    operands: { RP0: SP }
    tstates: [6]
    undocumented: false
  - encoding: DD 0B
    mnemonic: DEC BC
    operands: { RP0: BC }
    tstates: [10]
    undocumented: true
  - encoding: DD 1B
    mnemonic: DEC DE
    operands: { RP0: DE }
    tstates: [10]
    undocumented: true
  - encoding: DD 2B
    mnemonic: DEC IX
    operands: { RP0: IX }
    tstates: [10]
    undocumented: false
  - encoding: DD 3B
    mnemonic: DEC SP
    operands: { RP0: SP }
    tstates: [10]
    undocumented: true
  - encoding: FD 0B
    mnemonic: DEC BC
    operands: { RP0: BC }
    tstates: [10]
    undocumented: true
  - encoding: FD 1B
    mnemonic: DEC DE
    operands: { RP0: DE }
    tstates: [10]
    undocumented: true
  - encoding: FD 2B
    mnemonic: DEC IY
    operands: { RP0: IY }
    tstates: [10]
    undocumented: false
  - encoding: FD 3B
    mnemonic: DEC SP
    operands: { RP0: SP }
    tstates: [10]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# DEC rr

Decrements rr.

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
| `DEC BC` | `0B` | 6 |  |
| `DEC DE` | `1B` | 6 |  |
| `DEC HL` | `2B` | 6 |  |
| `DEC SP` | `3B` | 6 |  |
| `DEC BC` | `DD 0B` | 10 | Undocumented. |
| `DEC DE` | `DD 1B` | 10 | Undocumented. |
| `DEC IX` | `DD 2B` | 10 |  |
| `DEC SP` | `DD 3B` | 10 | Undocumented. |
| `DEC BC` | `FD 0B` | 10 | Undocumented. |
| `DEC DE` | `FD 1B` | 10 | Undocumented. |
| `DEC IY` | `FD 2B` | 10 |  |
| `DEC SP` | `FD 3B` | 10 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
