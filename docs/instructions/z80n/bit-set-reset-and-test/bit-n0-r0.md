---
cpu: Z80N
defined_by: Z80
form: BIT N0, R0
mnemonic: BIT b, r
group: Bit Set, Reset, and Test
description: "Tests bit {N0} of {R0}, setting the Z flag if it is zero."
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
    description: "Copied from bit 5 of {R0}."
  - flag: H
    effect: set
    description: Set.
  - flag: X
    effect: changed
    description: "Copied from bit 3 of {R0}."
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
  - encoding: CB 40
    mnemonic: BIT 0, B
    operands: { N0: 0, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 41
    mnemonic: BIT 0, C
    operands: { N0: 0, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 42
    mnemonic: BIT 0, D
    operands: { N0: 0, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 43
    mnemonic: BIT 0, E
    operands: { N0: 0, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 44
    mnemonic: BIT 0, H
    operands: { N0: 0, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 45
    mnemonic: BIT 0, L
    operands: { N0: 0, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 47
    mnemonic: BIT 0, A
    operands: { N0: 0, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB 48
    mnemonic: BIT 1, B
    operands: { N0: 1, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 49
    mnemonic: BIT 1, C
    operands: { N0: 1, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 4A
    mnemonic: BIT 1, D
    operands: { N0: 1, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 4B
    mnemonic: BIT 1, E
    operands: { N0: 1, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 4C
    mnemonic: BIT 1, H
    operands: { N0: 1, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 4D
    mnemonic: BIT 1, L
    operands: { N0: 1, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 4F
    mnemonic: BIT 1, A
    operands: { N0: 1, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB 50
    mnemonic: BIT 2, B
    operands: { N0: 2, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 51
    mnemonic: BIT 2, C
    operands: { N0: 2, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 52
    mnemonic: BIT 2, D
    operands: { N0: 2, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 53
    mnemonic: BIT 2, E
    operands: { N0: 2, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 54
    mnemonic: BIT 2, H
    operands: { N0: 2, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 55
    mnemonic: BIT 2, L
    operands: { N0: 2, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 57
    mnemonic: BIT 2, A
    operands: { N0: 2, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB 58
    mnemonic: BIT 3, B
    operands: { N0: 3, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 59
    mnemonic: BIT 3, C
    operands: { N0: 3, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 5A
    mnemonic: BIT 3, D
    operands: { N0: 3, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 5B
    mnemonic: BIT 3, E
    operands: { N0: 3, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 5C
    mnemonic: BIT 3, H
    operands: { N0: 3, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 5D
    mnemonic: BIT 3, L
    operands: { N0: 3, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 5F
    mnemonic: BIT 3, A
    operands: { N0: 3, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB 60
    mnemonic: BIT 4, B
    operands: { N0: 4, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 61
    mnemonic: BIT 4, C
    operands: { N0: 4, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 62
    mnemonic: BIT 4, D
    operands: { N0: 4, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 63
    mnemonic: BIT 4, E
    operands: { N0: 4, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 64
    mnemonic: BIT 4, H
    operands: { N0: 4, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 65
    mnemonic: BIT 4, L
    operands: { N0: 4, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 67
    mnemonic: BIT 4, A
    operands: { N0: 4, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB 68
    mnemonic: BIT 5, B
    operands: { N0: 5, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 69
    mnemonic: BIT 5, C
    operands: { N0: 5, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 6A
    mnemonic: BIT 5, D
    operands: { N0: 5, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 6B
    mnemonic: BIT 5, E
    operands: { N0: 5, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 6C
    mnemonic: BIT 5, H
    operands: { N0: 5, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 6D
    mnemonic: BIT 5, L
    operands: { N0: 5, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 6F
    mnemonic: BIT 5, A
    operands: { N0: 5, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB 70
    mnemonic: BIT 6, B
    operands: { N0: 6, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 71
    mnemonic: BIT 6, C
    operands: { N0: 6, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 72
    mnemonic: BIT 6, D
    operands: { N0: 6, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 73
    mnemonic: BIT 6, E
    operands: { N0: 6, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 74
    mnemonic: BIT 6, H
    operands: { N0: 6, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 75
    mnemonic: BIT 6, L
    operands: { N0: 6, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 77
    mnemonic: BIT 6, A
    operands: { N0: 6, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB 78
    mnemonic: BIT 7, B
    operands: { N0: 7, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 79
    mnemonic: BIT 7, C
    operands: { N0: 7, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 7A
    mnemonic: BIT 7, D
    operands: { N0: 7, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 7B
    mnemonic: BIT 7, E
    operands: { N0: 7, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 7C
    mnemonic: BIT 7, H
    operands: { N0: 7, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 7D
    mnemonic: BIT 7, L
    operands: { N0: 7, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 7F
    mnemonic: BIT 7, A
    operands: { N0: 7, R0: A }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# BIT b, r

Tests bit b of r, setting the Z flag if it is zero.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the tested bit. |
| Z | Set if the tested bit is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Set. |
| X | Copied from bit 3 of r. |
| PV | Set if the tested bit is zero, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `BIT 0, B` | `CB 40` | 8 |  |
| `BIT 0, C` | `CB 41` | 8 |  |
| `BIT 0, D` | `CB 42` | 8 |  |
| `BIT 0, E` | `CB 43` | 8 |  |
| `BIT 0, H` | `CB 44` | 8 |  |
| `BIT 0, L` | `CB 45` | 8 |  |
| `BIT 0, A` | `CB 47` | 8 |  |
| `BIT 1, B` | `CB 48` | 8 |  |
| `BIT 1, C` | `CB 49` | 8 |  |
| `BIT 1, D` | `CB 4A` | 8 |  |
| `BIT 1, E` | `CB 4B` | 8 |  |
| `BIT 1, H` | `CB 4C` | 8 |  |
| `BIT 1, L` | `CB 4D` | 8 |  |
| `BIT 1, A` | `CB 4F` | 8 |  |
| `BIT 2, B` | `CB 50` | 8 |  |
| `BIT 2, C` | `CB 51` | 8 |  |
| `BIT 2, D` | `CB 52` | 8 |  |
| `BIT 2, E` | `CB 53` | 8 |  |
| `BIT 2, H` | `CB 54` | 8 |  |
| `BIT 2, L` | `CB 55` | 8 |  |
| `BIT 2, A` | `CB 57` | 8 |  |
| `BIT 3, B` | `CB 58` | 8 |  |
| `BIT 3, C` | `CB 59` | 8 |  |
| `BIT 3, D` | `CB 5A` | 8 |  |
| `BIT 3, E` | `CB 5B` | 8 |  |
| `BIT 3, H` | `CB 5C` | 8 |  |
| `BIT 3, L` | `CB 5D` | 8 |  |
| `BIT 3, A` | `CB 5F` | 8 |  |
| `BIT 4, B` | `CB 60` | 8 |  |
| `BIT 4, C` | `CB 61` | 8 |  |
| `BIT 4, D` | `CB 62` | 8 |  |
| `BIT 4, E` | `CB 63` | 8 |  |
| `BIT 4, H` | `CB 64` | 8 |  |
| `BIT 4, L` | `CB 65` | 8 |  |
| `BIT 4, A` | `CB 67` | 8 |  |
| `BIT 5, B` | `CB 68` | 8 |  |
| `BIT 5, C` | `CB 69` | 8 |  |
| `BIT 5, D` | `CB 6A` | 8 |  |
| `BIT 5, E` | `CB 6B` | 8 |  |
| `BIT 5, H` | `CB 6C` | 8 |  |
| `BIT 5, L` | `CB 6D` | 8 |  |
| `BIT 5, A` | `CB 6F` | 8 |  |
| `BIT 6, B` | `CB 70` | 8 |  |
| `BIT 6, C` | `CB 71` | 8 |  |
| `BIT 6, D` | `CB 72` | 8 |  |
| `BIT 6, E` | `CB 73` | 8 |  |
| `BIT 6, H` | `CB 74` | 8 |  |
| `BIT 6, L` | `CB 75` | 8 |  |
| `BIT 6, A` | `CB 77` | 8 |  |
| `BIT 7, B` | `CB 78` | 8 |  |
| `BIT 7, C` | `CB 79` | 8 |  |
| `BIT 7, D` | `CB 7A` | 8 |  |
| `BIT 7, E` | `CB 7B` | 8 |  |
| `BIT 7, H` | `CB 7C` | 8 |  |
| `BIT 7, L` | `CB 7D` | 8 |  |
| `BIT 7, A` | `CB 7F` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
