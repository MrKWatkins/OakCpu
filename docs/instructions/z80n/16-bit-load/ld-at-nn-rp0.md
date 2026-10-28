---
cpu: Z80N
defined_by: Z80
form: LD (nn), RP0
mnemonic: LD (nn), rr
group: 16-Bit Load
description: "Stores {RP0} at address nn, low byte first."
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
  - encoding: 22 nn
    mnemonic: LD (nn), HL
    operands: { RP0: HL }
    tstates: [16]
    undocumented: false
  - encoding: DD 22 nn
    mnemonic: LD (nn), IX
    operands: { RP0: IX }
    tstates: [20]
    undocumented: false
  - encoding: ED 43 nn
    mnemonic: LD (nn), BC
    operands: { RP0: BC }
    tstates: [20]
    undocumented: false
  - encoding: ED 53 nn
    mnemonic: LD (nn), DE
    operands: { RP0: DE }
    tstates: [20]
    undocumented: false
  - encoding: ED 63 nn
    mnemonic: LD (nn), HL
    operands: { RP0: HL }
    tstates: [20]
    undocumented: true
  - encoding: ED 73 nn
    mnemonic: LD (nn), SP
    operands: { RP0: SP }
    tstates: [20]
    undocumented: false
  - encoding: FD 22 nn
    mnemonic: LD (nn), IY
    operands: { RP0: IY }
    tstates: [20]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD (nn), rr

Stores rr at address nn, low byte first.

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
| `LD (nn), HL` | `22 nn` | 16 |  |
| `LD (nn), IX` | `DD 22 nn` | 20 |  |
| `LD (nn), BC` | `ED 43 nn` | 20 |  |
| `LD (nn), DE` | `ED 53 nn` | 20 |  |
| `LD (nn), HL` | `ED 63 nn` | 20 | Undocumented. |
| `LD (nn), SP` | `ED 73 nn` | 20 |  |
| `LD (nn), IY` | `FD 22 nn` | 20 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
