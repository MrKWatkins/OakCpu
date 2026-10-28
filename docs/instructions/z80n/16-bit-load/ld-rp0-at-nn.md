---
cpu: Z80N
defined_by: Z80
form: LD RP0, (nn)
mnemonic: LD rr, (nn)
group: 16-Bit Load
description: "Loads the word at address nn into {RP0}, low byte first."
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
  - encoding: 2A nn
    mnemonic: LD HL, (nn)
    operands: { RP0: HL }
    tstates: [16]
    undocumented: false
  - encoding: DD 2A nn
    mnemonic: LD IX, (nn)
    operands: { RP0: IX }
    tstates: [20]
    undocumented: false
  - encoding: ED 4B nn
    mnemonic: LD BC, (nn)
    operands: { RP0: BC }
    tstates: [20]
    undocumented: false
  - encoding: ED 5B nn
    mnemonic: LD DE, (nn)
    operands: { RP0: DE }
    tstates: [20]
    undocumented: false
  - encoding: ED 6B nn
    mnemonic: LD HL, (nn)
    operands: { RP0: HL }
    tstates: [20]
    undocumented: true
  - encoding: ED 7B nn
    mnemonic: LD SP, (nn)
    operands: { RP0: SP }
    tstates: [20]
    undocumented: false
  - encoding: FD 2A nn
    mnemonic: LD IY, (nn)
    operands: { RP0: IY }
    tstates: [20]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD rr, (nn)

Loads the word at address nn into rr, low byte first.

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
| `LD HL, (nn)` | `2A nn` | 16 |  |
| `LD IX, (nn)` | `DD 2A nn` | 20 |  |
| `LD BC, (nn)` | `ED 4B nn` | 20 |  |
| `LD DE, (nn)` | `ED 5B nn` | 20 |  |
| `LD HL, (nn)` | `ED 6B nn` | 20 | Undocumented. |
| `LD SP, (nn)` | `ED 7B nn` | 20 |  |
| `LD IY, (nn)` | `FD 2A nn` | 20 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
