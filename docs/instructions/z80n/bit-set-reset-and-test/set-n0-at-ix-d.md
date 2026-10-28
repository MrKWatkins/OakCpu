---
cpu: Z80N
defined_by: Z80
form: SET N0, (IX + d)
mnemonic: SET b, (IX + d)
group: Bit Set, Reset, and Test
description: "Sets bit {N0} of the byte at (IX + d)."
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
  - encoding: DD CB d C6
    mnemonic: SET 0, (IX + d)
    operands: { N0: 0 }
    tstates: [23]
    undocumented: false
  - encoding: DD CB d CE
    mnemonic: SET 1, (IX + d)
    operands: { N0: 1 }
    tstates: [23]
    undocumented: false
  - encoding: DD CB d D6
    mnemonic: SET 2, (IX + d)
    operands: { N0: 2 }
    tstates: [23]
    undocumented: false
  - encoding: DD CB d DE
    mnemonic: SET 3, (IX + d)
    operands: { N0: 3 }
    tstates: [23]
    undocumented: false
  - encoding: DD CB d E6
    mnemonic: SET 4, (IX + d)
    operands: { N0: 4 }
    tstates: [23]
    undocumented: false
  - encoding: DD CB d EE
    mnemonic: SET 5, (IX + d)
    operands: { N0: 5 }
    tstates: [23]
    undocumented: false
  - encoding: DD CB d F6
    mnemonic: SET 6, (IX + d)
    operands: { N0: 6 }
    tstates: [23]
    undocumented: false
  - encoding: DD CB d FE
    mnemonic: SET 7, (IX + d)
    operands: { N0: 7 }
    tstates: [23]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SET b, (IX + d)

Sets bit b of the byte at (IX + d).

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
| `SET 0, (IX + d)` | `DD CB d C6` | 23 |  |
| `SET 1, (IX + d)` | `DD CB d CE` | 23 |  |
| `SET 2, (IX + d)` | `DD CB d D6` | 23 |  |
| `SET 3, (IX + d)` | `DD CB d DE` | 23 |  |
| `SET 4, (IX + d)` | `DD CB d E6` | 23 |  |
| `SET 5, (IX + d)` | `DD CB d EE` | 23 |  |
| `SET 6, (IX + d)` | `DD CB d F6` | 23 |  |
| `SET 7, (IX + d)` | `DD CB d FE` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
