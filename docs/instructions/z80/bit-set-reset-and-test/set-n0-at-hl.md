---
cpu: Z80
defined_by: Z80
form: SET N0, (HL)
mnemonic: SET b, (HL)
group: Bit Set, Reset, and Test
description: "Sets bit {N0} of the byte at (HL)."
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
  - encoding: CB C6
    mnemonic: SET 0, (HL)
    operands: { N0: 0 }
    tstates: [15]
    undocumented: false
  - encoding: CB CE
    mnemonic: SET 1, (HL)
    operands: { N0: 1 }
    tstates: [15]
    undocumented: false
  - encoding: CB D6
    mnemonic: SET 2, (HL)
    operands: { N0: 2 }
    tstates: [15]
    undocumented: false
  - encoding: CB DE
    mnemonic: SET 3, (HL)
    operands: { N0: 3 }
    tstates: [15]
    undocumented: false
  - encoding: CB E6
    mnemonic: SET 4, (HL)
    operands: { N0: 4 }
    tstates: [15]
    undocumented: false
  - encoding: CB EE
    mnemonic: SET 5, (HL)
    operands: { N0: 5 }
    tstates: [15]
    undocumented: false
  - encoding: CB F6
    mnemonic: SET 6, (HL)
    operands: { N0: 6 }
    tstates: [15]
    undocumented: false
  - encoding: CB FE
    mnemonic: SET 7, (HL)
    operands: { N0: 7 }
    tstates: [15]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SET b, (HL)

Sets bit b of the byte at (HL).

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
| `SET 0, (HL)` | `CB C6` | 15 |  |
| `SET 1, (HL)` | `CB CE` | 15 |  |
| `SET 2, (HL)` | `CB D6` | 15 |  |
| `SET 3, (HL)` | `CB DE` | 15 |  |
| `SET 4, (HL)` | `CB E6` | 15 |  |
| `SET 5, (HL)` | `CB EE` | 15 |  |
| `SET 6, (HL)` | `CB F6` | 15 |  |
| `SET 7, (HL)` | `CB FE` | 15 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
