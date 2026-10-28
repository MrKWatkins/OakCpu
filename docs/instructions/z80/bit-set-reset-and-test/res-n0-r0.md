---
cpu: Z80
defined_by: Z80
form: RES N0, R0
mnemonic: RES b, r
group: Bit Set, Reset, and Test
description: "Resets bit {N0} of {R0}."
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
  - encoding: CB 80
    mnemonic: RES 0, B
    operands: { N0: 0, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 81
    mnemonic: RES 0, C
    operands: { N0: 0, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 82
    mnemonic: RES 0, D
    operands: { N0: 0, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 83
    mnemonic: RES 0, E
    operands: { N0: 0, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 84
    mnemonic: RES 0, H
    operands: { N0: 0, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 85
    mnemonic: RES 0, L
    operands: { N0: 0, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 87
    mnemonic: RES 0, A
    operands: { N0: 0, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB 88
    mnemonic: RES 1, B
    operands: { N0: 1, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 89
    mnemonic: RES 1, C
    operands: { N0: 1, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 8A
    mnemonic: RES 1, D
    operands: { N0: 1, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 8B
    mnemonic: RES 1, E
    operands: { N0: 1, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 8C
    mnemonic: RES 1, H
    operands: { N0: 1, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 8D
    mnemonic: RES 1, L
    operands: { N0: 1, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 8F
    mnemonic: RES 1, A
    operands: { N0: 1, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB 90
    mnemonic: RES 2, B
    operands: { N0: 2, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 91
    mnemonic: RES 2, C
    operands: { N0: 2, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 92
    mnemonic: RES 2, D
    operands: { N0: 2, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 93
    mnemonic: RES 2, E
    operands: { N0: 2, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 94
    mnemonic: RES 2, H
    operands: { N0: 2, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 95
    mnemonic: RES 2, L
    operands: { N0: 2, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 97
    mnemonic: RES 2, A
    operands: { N0: 2, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB 98
    mnemonic: RES 3, B
    operands: { N0: 3, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB 99
    mnemonic: RES 3, C
    operands: { N0: 3, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB 9A
    mnemonic: RES 3, D
    operands: { N0: 3, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB 9B
    mnemonic: RES 3, E
    operands: { N0: 3, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB 9C
    mnemonic: RES 3, H
    operands: { N0: 3, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB 9D
    mnemonic: RES 3, L
    operands: { N0: 3, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB 9F
    mnemonic: RES 3, A
    operands: { N0: 3, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB A0
    mnemonic: RES 4, B
    operands: { N0: 4, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB A1
    mnemonic: RES 4, C
    operands: { N0: 4, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB A2
    mnemonic: RES 4, D
    operands: { N0: 4, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB A3
    mnemonic: RES 4, E
    operands: { N0: 4, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB A4
    mnemonic: RES 4, H
    operands: { N0: 4, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB A5
    mnemonic: RES 4, L
    operands: { N0: 4, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB A7
    mnemonic: RES 4, A
    operands: { N0: 4, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB A8
    mnemonic: RES 5, B
    operands: { N0: 5, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB A9
    mnemonic: RES 5, C
    operands: { N0: 5, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB AA
    mnemonic: RES 5, D
    operands: { N0: 5, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB AB
    mnemonic: RES 5, E
    operands: { N0: 5, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB AC
    mnemonic: RES 5, H
    operands: { N0: 5, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB AD
    mnemonic: RES 5, L
    operands: { N0: 5, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB AF
    mnemonic: RES 5, A
    operands: { N0: 5, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB B0
    mnemonic: RES 6, B
    operands: { N0: 6, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB B1
    mnemonic: RES 6, C
    operands: { N0: 6, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB B2
    mnemonic: RES 6, D
    operands: { N0: 6, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB B3
    mnemonic: RES 6, E
    operands: { N0: 6, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB B4
    mnemonic: RES 6, H
    operands: { N0: 6, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB B5
    mnemonic: RES 6, L
    operands: { N0: 6, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB B7
    mnemonic: RES 6, A
    operands: { N0: 6, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB B8
    mnemonic: RES 7, B
    operands: { N0: 7, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB B9
    mnemonic: RES 7, C
    operands: { N0: 7, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB BA
    mnemonic: RES 7, D
    operands: { N0: 7, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB BB
    mnemonic: RES 7, E
    operands: { N0: 7, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB BC
    mnemonic: RES 7, H
    operands: { N0: 7, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB BD
    mnemonic: RES 7, L
    operands: { N0: 7, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB BF
    mnemonic: RES 7, A
    operands: { N0: 7, R0: A }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RES b, r

Resets bit b of r.

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
| `RES 0, B` | `CB 80` | 8 |  |
| `RES 0, C` | `CB 81` | 8 |  |
| `RES 0, D` | `CB 82` | 8 |  |
| `RES 0, E` | `CB 83` | 8 |  |
| `RES 0, H` | `CB 84` | 8 |  |
| `RES 0, L` | `CB 85` | 8 |  |
| `RES 0, A` | `CB 87` | 8 |  |
| `RES 1, B` | `CB 88` | 8 |  |
| `RES 1, C` | `CB 89` | 8 |  |
| `RES 1, D` | `CB 8A` | 8 |  |
| `RES 1, E` | `CB 8B` | 8 |  |
| `RES 1, H` | `CB 8C` | 8 |  |
| `RES 1, L` | `CB 8D` | 8 |  |
| `RES 1, A` | `CB 8F` | 8 |  |
| `RES 2, B` | `CB 90` | 8 |  |
| `RES 2, C` | `CB 91` | 8 |  |
| `RES 2, D` | `CB 92` | 8 |  |
| `RES 2, E` | `CB 93` | 8 |  |
| `RES 2, H` | `CB 94` | 8 |  |
| `RES 2, L` | `CB 95` | 8 |  |
| `RES 2, A` | `CB 97` | 8 |  |
| `RES 3, B` | `CB 98` | 8 |  |
| `RES 3, C` | `CB 99` | 8 |  |
| `RES 3, D` | `CB 9A` | 8 |  |
| `RES 3, E` | `CB 9B` | 8 |  |
| `RES 3, H` | `CB 9C` | 8 |  |
| `RES 3, L` | `CB 9D` | 8 |  |
| `RES 3, A` | `CB 9F` | 8 |  |
| `RES 4, B` | `CB A0` | 8 |  |
| `RES 4, C` | `CB A1` | 8 |  |
| `RES 4, D` | `CB A2` | 8 |  |
| `RES 4, E` | `CB A3` | 8 |  |
| `RES 4, H` | `CB A4` | 8 |  |
| `RES 4, L` | `CB A5` | 8 |  |
| `RES 4, A` | `CB A7` | 8 |  |
| `RES 5, B` | `CB A8` | 8 |  |
| `RES 5, C` | `CB A9` | 8 |  |
| `RES 5, D` | `CB AA` | 8 |  |
| `RES 5, E` | `CB AB` | 8 |  |
| `RES 5, H` | `CB AC` | 8 |  |
| `RES 5, L` | `CB AD` | 8 |  |
| `RES 5, A` | `CB AF` | 8 |  |
| `RES 6, B` | `CB B0` | 8 |  |
| `RES 6, C` | `CB B1` | 8 |  |
| `RES 6, D` | `CB B2` | 8 |  |
| `RES 6, E` | `CB B3` | 8 |  |
| `RES 6, H` | `CB B4` | 8 |  |
| `RES 6, L` | `CB B5` | 8 |  |
| `RES 6, A` | `CB B7` | 8 |  |
| `RES 7, B` | `CB B8` | 8 |  |
| `RES 7, C` | `CB B9` | 8 |  |
| `RES 7, D` | `CB BA` | 8 |  |
| `RES 7, E` | `CB BB` | 8 |  |
| `RES 7, H` | `CB BC` | 8 |  |
| `RES 7, L` | `CB BD` | 8 |  |
| `RES 7, A` | `CB BF` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
