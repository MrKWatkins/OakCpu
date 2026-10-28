---
cpu: Z80
defined_by: Z80
form: LD RP0, nn
mnemonic: LD rr, nn
group: 16-Bit Load
description: "Loads nn into {RP0}."
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
  - encoding: 01 nn
    mnemonic: LD BC, nn
    operands: { RP0: BC }
    tstates: [10]
    undocumented: false
  - encoding: 11 nn
    mnemonic: LD DE, nn
    operands: { RP0: DE }
    tstates: [10]
    undocumented: false
  - encoding: 21 nn
    mnemonic: LD HL, nn
    operands: { RP0: HL }
    tstates: [10]
    undocumented: false
  - encoding: 31 nn
    mnemonic: LD SP, nn
    operands: { RP0: SP }
    tstates: [10]
    undocumented: false
  - encoding: DD 01 nn
    mnemonic: LD BC, nn
    operands: { RP0: BC }
    tstates: [14]
    undocumented: true
  - encoding: DD 11 nn
    mnemonic: LD DE, nn
    operands: { RP0: DE }
    tstates: [14]
    undocumented: true
  - encoding: DD 21 nn
    mnemonic: LD IX, nn
    operands: { RP0: IX }
    tstates: [14]
    undocumented: false
  - encoding: DD 31 nn
    mnemonic: LD SP, nn
    operands: { RP0: SP }
    tstates: [14]
    undocumented: true
  - encoding: FD 01 nn
    mnemonic: LD BC, nn
    operands: { RP0: BC }
    tstates: [14]
    undocumented: true
  - encoding: FD 11 nn
    mnemonic: LD DE, nn
    operands: { RP0: DE }
    tstates: [14]
    undocumented: true
  - encoding: FD 21 nn
    mnemonic: LD IY, nn
    operands: { RP0: IY }
    tstates: [14]
    undocumented: false
  - encoding: FD 31 nn
    mnemonic: LD SP, nn
    operands: { RP0: SP }
    tstates: [14]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD rr, nn

Loads nn into rr.

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
| `LD BC, nn` | `01 nn` | 10 |  |
| `LD DE, nn` | `11 nn` | 10 |  |
| `LD HL, nn` | `21 nn` | 10 |  |
| `LD SP, nn` | `31 nn` | 10 |  |
| `LD BC, nn` | `DD 01 nn` | 14 | Undocumented. |
| `LD DE, nn` | `DD 11 nn` | 14 | Undocumented. |
| `LD IX, nn` | `DD 21 nn` | 14 |  |
| `LD SP, nn` | `DD 31 nn` | 14 | Undocumented. |
| `LD BC, nn` | `FD 01 nn` | 14 | Undocumented. |
| `LD DE, nn` | `FD 11 nn` | 14 | Undocumented. |
| `LD IY, nn` | `FD 21 nn` | 14 |  |
| `LD SP, nn` | `FD 31 nn` | 14 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
