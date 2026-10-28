---
cpu: Z80N
defined_by: Z80
form: SET N0, (IY + d)
mnemonic: SET b, (IY + d)
group: Bit Set, Reset, and Test
description: "Sets bit {N0} of the byte at (IY + d)."
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
  - encoding: FD CB d C6
    mnemonic: SET 0, (IY + d)
    operands: { N0: 0 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d CE
    mnemonic: SET 1, (IY + d)
    operands: { N0: 1 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d D6
    mnemonic: SET 2, (IY + d)
    operands: { N0: 2 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d DE
    mnemonic: SET 3, (IY + d)
    operands: { N0: 3 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d E6
    mnemonic: SET 4, (IY + d)
    operands: { N0: 4 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d EE
    mnemonic: SET 5, (IY + d)
    operands: { N0: 5 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d F6
    mnemonic: SET 6, (IY + d)
    operands: { N0: 6 }
    tstates: [23]
    undocumented: false
  - encoding: FD CB d FE
    mnemonic: SET 7, (IY + d)
    operands: { N0: 7 }
    tstates: [23]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SET b, (IY + d)

Sets bit b of the byte at (IY + d).

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
| `SET 0, (IY + d)` | `FD CB d C6` | 23 |  |
| `SET 1, (IY + d)` | `FD CB d CE` | 23 |  |
| `SET 2, (IY + d)` | `FD CB d D6` | 23 |  |
| `SET 3, (IY + d)` | `FD CB d DE` | 23 |  |
| `SET 4, (IY + d)` | `FD CB d E6` | 23 |  |
| `SET 5, (IY + d)` | `FD CB d EE` | 23 |  |
| `SET 6, (IY + d)` | `FD CB d F6` | 23 |  |
| `SET 7, (IY + d)` | `FD CB d FE` | 23 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
