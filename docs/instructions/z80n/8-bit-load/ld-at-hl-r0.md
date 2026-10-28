---
cpu: Z80N
defined_by: Z80
form: LD (HL), R0
mnemonic: LD (HL), r
group: 8-Bit Load
description: "Stores {R0} at (HL)."
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
  - encoding: 70
    mnemonic: LD (HL), B
    operands: { R0: B }
    tstates: [7]
    undocumented: false
  - encoding: 71
    mnemonic: LD (HL), C
    operands: { R0: C }
    tstates: [7]
    undocumented: false
  - encoding: 72
    mnemonic: LD (HL), D
    operands: { R0: D }
    tstates: [7]
    undocumented: false
  - encoding: 73
    mnemonic: LD (HL), E
    operands: { R0: E }
    tstates: [7]
    undocumented: false
  - encoding: 74
    mnemonic: LD (HL), H
    operands: { R0: H }
    tstates: [7]
    undocumented: false
  - encoding: 75
    mnemonic: LD (HL), L
    operands: { R0: L }
    tstates: [7]
    undocumented: false
  - encoding: 77
    mnemonic: LD (HL), A
    operands: { R0: A }
    tstates: [7]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD (HL), r

Stores r at (HL).

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
| `LD (HL), B` | `70` | 7 |  |
| `LD (HL), C` | `71` | 7 |  |
| `LD (HL), D` | `72` | 7 |  |
| `LD (HL), E` | `73` | 7 |  |
| `LD (HL), H` | `74` | 7 |  |
| `LD (HL), L` | `75` | 7 |  |
| `LD (HL), A` | `77` | 7 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
