---
cpu: Z80
defined_by: Z80
form: RES N0, (HL)
mnemonic: RES b, (HL)
group: Bit Set, Reset, and Test
description: "Resets bit {N0} of the byte at (HL)."
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
  - encoding: CB 86
    mnemonic: RES 0, (HL)
    operands: { N0: 0 }
    tstates: [15]
    undocumented: false
  - encoding: CB 8E
    mnemonic: RES 1, (HL)
    operands: { N0: 1 }
    tstates: [15]
    undocumented: false
  - encoding: CB 96
    mnemonic: RES 2, (HL)
    operands: { N0: 2 }
    tstates: [15]
    undocumented: false
  - encoding: CB 9E
    mnemonic: RES 3, (HL)
    operands: { N0: 3 }
    tstates: [15]
    undocumented: false
  - encoding: CB A6
    mnemonic: RES 4, (HL)
    operands: { N0: 4 }
    tstates: [15]
    undocumented: false
  - encoding: CB AE
    mnemonic: RES 5, (HL)
    operands: { N0: 5 }
    tstates: [15]
    undocumented: false
  - encoding: CB B6
    mnemonic: RES 6, (HL)
    operands: { N0: 6 }
    tstates: [15]
    undocumented: false
  - encoding: CB BE
    mnemonic: RES 7, (HL)
    operands: { N0: 7 }
    tstates: [15]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RES b, (HL)

Resets bit b of the byte at (HL).

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
| `RES 0, (HL)` | `CB 86` | 15 |  |
| `RES 1, (HL)` | `CB 8E` | 15 |  |
| `RES 2, (HL)` | `CB 96` | 15 |  |
| `RES 3, (HL)` | `CB 9E` | 15 |  |
| `RES 4, (HL)` | `CB A6` | 15 |  |
| `RES 5, (HL)` | `CB AE` | 15 |  |
| `RES 6, (HL)` | `CB B6` | 15 |  |
| `RES 7, (HL)` | `CB BE` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
