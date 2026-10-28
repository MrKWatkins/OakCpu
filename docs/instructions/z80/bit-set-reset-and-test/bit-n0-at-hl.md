---
cpu: Z80
defined_by: Z80
form: BIT N0, (HL)
mnemonic: BIT b, (HL)
group: Bit Set, Reset, and Test
description: "Tests bit {N0} of the byte at (HL), setting the Z flag if it is zero."
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the tested bit.
  - flag: Z
    effect: changed
    description: Set if the tested bit is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of W.
  - flag: H
    effect: set
    description: Set.
  - flag: X
    effect: changed
    description: Copied from bit 3 of W.
  - flag: PV
    effect: changed
    description: Set if the tested bit is zero, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: CB 46
    mnemonic: BIT 0, (HL)
    operands: { N0: 0 }
    tstates: [12]
    undocumented: false
  - encoding: CB 4E
    mnemonic: BIT 1, (HL)
    operands: { N0: 1 }
    tstates: [12]
    undocumented: false
  - encoding: CB 56
    mnemonic: BIT 2, (HL)
    operands: { N0: 2 }
    tstates: [12]
    undocumented: false
  - encoding: CB 5E
    mnemonic: BIT 3, (HL)
    operands: { N0: 3 }
    tstates: [12]
    undocumented: false
  - encoding: CB 66
    mnemonic: BIT 4, (HL)
    operands: { N0: 4 }
    tstates: [12]
    undocumented: false
  - encoding: CB 6E
    mnemonic: BIT 5, (HL)
    operands: { N0: 5 }
    tstates: [12]
    undocumented: false
  - encoding: CB 76
    mnemonic: BIT 6, (HL)
    operands: { N0: 6 }
    tstates: [12]
    undocumented: false
  - encoding: CB 7E
    mnemonic: BIT 7, (HL)
    operands: { N0: 7 }
    tstates: [12]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# BIT b, (HL)

Tests bit b of the byte at (HL), setting the Z flag if it is zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the tested bit. |
| Z | Set if the tested bit is zero, otherwise reset. |
| Y | Copied from bit 5 of W. |
| H | Set. |
| X | Copied from bit 3 of W. |
| PV | Set if the tested bit is zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `BIT 0, (HL)` | `CB 46` | 12 |  |
| `BIT 1, (HL)` | `CB 4E` | 12 |  |
| `BIT 2, (HL)` | `CB 56` | 12 |  |
| `BIT 3, (HL)` | `CB 5E` | 12 |  |
| `BIT 4, (HL)` | `CB 66` | 12 |  |
| `BIT 5, (HL)` | `CB 6E` | 12 |  |
| `BIT 6, (HL)` | `CB 76` | 12 |  |
| `BIT 7, (HL)` | `CB 7E` | 12 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
