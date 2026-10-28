---
cpu: Z80
defined_by: Z80
form: RES N0, (IY + d)
mnemonic: RES b, (IY + d)
group: Bit Set, Reset, and Test
description: "Resets bit {N0} of the byte at (IY + d)."
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
  - encoding: FD CB d 86
    mnemonic: RES 0, (IY + d)
    operands: { N0: 0 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d 8E
    mnemonic: RES 1, (IY + d)
    operands: { N0: 1 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d 96
    mnemonic: RES 2, (IY + d)
    operands: { N0: 2 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d 9E
    mnemonic: RES 3, (IY + d)
    operands: { N0: 3 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d A6
    mnemonic: RES 4, (IY + d)
    operands: { N0: 4 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d AE
    mnemonic: RES 5, (IY + d)
    operands: { N0: 5 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d B6
    mnemonic: RES 6, (IY + d)
    operands: { N0: 6 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d BE
    mnemonic: RES 7, (IY + d)
    operands: { N0: 7 }
    tstates: [23]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RES b, (IY + d)

Resets bit b of the byte at (IY + d).

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
| `RES 0, (IY + d)` | `FD CB d 86` | 23 |  |
| `RES 1, (IY + d)` | `FD CB d 8E` | 23 |  |
| `RES 2, (IY + d)` | `FD CB d 96` | 23 |  |
| `RES 3, (IY + d)` | `FD CB d 9E` | 23 |  |
| `RES 4, (IY + d)` | `FD CB d A6` | 23 |  |
| `RES 5, (IY + d)` | `FD CB d AE` | 23 |  |
| `RES 6, (IY + d)` | `FD CB d B6` | 23 |  |
| `RES 7, (IY + d)` | `FD CB d BE` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
