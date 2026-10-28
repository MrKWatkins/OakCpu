---
cpu: Z80N
defined_by: Z80
form: RES N0, (IX + d), R0
mnemonic: RES b, (IX + d), r
group: Bit Set, Reset, and Test
description: "Resets bit {N0} of the byte at (IX + d), storing the result in both the byte at (IX + d) and {R0}."
undocumented: true
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
  - encoding: DD CB d 80
    mnemonic: RES 0, (IX + d), B
    operands: { N0: 0, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 81
    mnemonic: RES 0, (IX + d), C
    operands: { N0: 0, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 82
    mnemonic: RES 0, (IX + d), D
    operands: { N0: 0, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 83
    mnemonic: RES 0, (IX + d), E
    operands: { N0: 0, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 84
    mnemonic: RES 0, (IX + d), H
    operands: { N0: 0, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 85
    mnemonic: RES 0, (IX + d), L
    operands: { N0: 0, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 87
    mnemonic: RES 0, (IX + d), A
    operands: { N0: 0, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 88
    mnemonic: RES 1, (IX + d), B
    operands: { N0: 1, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 89
    mnemonic: RES 1, (IX + d), C
    operands: { N0: 1, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 8A
    mnemonic: RES 1, (IX + d), D
    operands: { N0: 1, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 8B
    mnemonic: RES 1, (IX + d), E
    operands: { N0: 1, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 8C
    mnemonic: RES 1, (IX + d), H
    operands: { N0: 1, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 8D
    mnemonic: RES 1, (IX + d), L
    operands: { N0: 1, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 8F
    mnemonic: RES 1, (IX + d), A
    operands: { N0: 1, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 90
    mnemonic: RES 2, (IX + d), B
    operands: { N0: 2, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 91
    mnemonic: RES 2, (IX + d), C
    operands: { N0: 2, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 92
    mnemonic: RES 2, (IX + d), D
    operands: { N0: 2, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 93
    mnemonic: RES 2, (IX + d), E
    operands: { N0: 2, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 94
    mnemonic: RES 2, (IX + d), H
    operands: { N0: 2, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 95
    mnemonic: RES 2, (IX + d), L
    operands: { N0: 2, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 97
    mnemonic: RES 2, (IX + d), A
    operands: { N0: 2, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 98
    mnemonic: RES 3, (IX + d), B
    operands: { N0: 3, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 99
    mnemonic: RES 3, (IX + d), C
    operands: { N0: 3, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 9A
    mnemonic: RES 3, (IX + d), D
    operands: { N0: 3, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 9B
    mnemonic: RES 3, (IX + d), E
    operands: { N0: 3, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 9C
    mnemonic: RES 3, (IX + d), H
    operands: { N0: 3, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 9D
    mnemonic: RES 3, (IX + d), L
    operands: { N0: 3, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d 9F
    mnemonic: RES 3, (IX + d), A
    operands: { N0: 3, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d A0
    mnemonic: RES 4, (IX + d), B
    operands: { N0: 4, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d A1
    mnemonic: RES 4, (IX + d), C
    operands: { N0: 4, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d A2
    mnemonic: RES 4, (IX + d), D
    operands: { N0: 4, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d A3
    mnemonic: RES 4, (IX + d), E
    operands: { N0: 4, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d A4
    mnemonic: RES 4, (IX + d), H
    operands: { N0: 4, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d A5
    mnemonic: RES 4, (IX + d), L
    operands: { N0: 4, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d A7
    mnemonic: RES 4, (IX + d), A
    operands: { N0: 4, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d A8
    mnemonic: RES 5, (IX + d), B
    operands: { N0: 5, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d A9
    mnemonic: RES 5, (IX + d), C
    operands: { N0: 5, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d AA
    mnemonic: RES 5, (IX + d), D
    operands: { N0: 5, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d AB
    mnemonic: RES 5, (IX + d), E
    operands: { N0: 5, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d AC
    mnemonic: RES 5, (IX + d), H
    operands: { N0: 5, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d AD
    mnemonic: RES 5, (IX + d), L
    operands: { N0: 5, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d AF
    mnemonic: RES 5, (IX + d), A
    operands: { N0: 5, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d B0
    mnemonic: RES 6, (IX + d), B
    operands: { N0: 6, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d B1
    mnemonic: RES 6, (IX + d), C
    operands: { N0: 6, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d B2
    mnemonic: RES 6, (IX + d), D
    operands: { N0: 6, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d B3
    mnemonic: RES 6, (IX + d), E
    operands: { N0: 6, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d B4
    mnemonic: RES 6, (IX + d), H
    operands: { N0: 6, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d B5
    mnemonic: RES 6, (IX + d), L
    operands: { N0: 6, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d B7
    mnemonic: RES 6, (IX + d), A
    operands: { N0: 6, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d B8
    mnemonic: RES 7, (IX + d), B
    operands: { N0: 7, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d B9
    mnemonic: RES 7, (IX + d), C
    operands: { N0: 7, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d BA
    mnemonic: RES 7, (IX + d), D
    operands: { N0: 7, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d BB
    mnemonic: RES 7, (IX + d), E
    operands: { N0: 7, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d BC
    mnemonic: RES 7, (IX + d), H
    operands: { N0: 7, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d BD
    mnemonic: RES 7, (IX + d), L
    operands: { N0: 7, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: DD CB d BF
    mnemonic: RES 7, (IX + d), A
    operands: { N0: 7, R0: A }
    tstates: [23]
    undocumented: true
references:
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
  - name: The Undocumented Z80 Documented
    url: "http://www.z80.info/zip/z80-documented.pdf"
---

# RES b, (IX + d), r

Resets bit b of the byte at (IX + d), storing the result in both the byte at (IX + d) and r.

This instruction is undocumented.

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
| `RES 0, (IX + d), B` | `DD CB d 80` | 23 |  |
| `RES 0, (IX + d), C` | `DD CB d 81` | 23 |  |
| `RES 0, (IX + d), D` | `DD CB d 82` | 23 |  |
| `RES 0, (IX + d), E` | `DD CB d 83` | 23 |  |
| `RES 0, (IX + d), H` | `DD CB d 84` | 23 |  |
| `RES 0, (IX + d), L` | `DD CB d 85` | 23 |  |
| `RES 0, (IX + d), A` | `DD CB d 87` | 23 |  |
| `RES 1, (IX + d), B` | `DD CB d 88` | 23 |  |
| `RES 1, (IX + d), C` | `DD CB d 89` | 23 |  |
| `RES 1, (IX + d), D` | `DD CB d 8A` | 23 |  |
| `RES 1, (IX + d), E` | `DD CB d 8B` | 23 |  |
| `RES 1, (IX + d), H` | `DD CB d 8C` | 23 |  |
| `RES 1, (IX + d), L` | `DD CB d 8D` | 23 |  |
| `RES 1, (IX + d), A` | `DD CB d 8F` | 23 |  |
| `RES 2, (IX + d), B` | `DD CB d 90` | 23 |  |
| `RES 2, (IX + d), C` | `DD CB d 91` | 23 |  |
| `RES 2, (IX + d), D` | `DD CB d 92` | 23 |  |
| `RES 2, (IX + d), E` | `DD CB d 93` | 23 |  |
| `RES 2, (IX + d), H` | `DD CB d 94` | 23 |  |
| `RES 2, (IX + d), L` | `DD CB d 95` | 23 |  |
| `RES 2, (IX + d), A` | `DD CB d 97` | 23 |  |
| `RES 3, (IX + d), B` | `DD CB d 98` | 23 |  |
| `RES 3, (IX + d), C` | `DD CB d 99` | 23 |  |
| `RES 3, (IX + d), D` | `DD CB d 9A` | 23 |  |
| `RES 3, (IX + d), E` | `DD CB d 9B` | 23 |  |
| `RES 3, (IX + d), H` | `DD CB d 9C` | 23 |  |
| `RES 3, (IX + d), L` | `DD CB d 9D` | 23 |  |
| `RES 3, (IX + d), A` | `DD CB d 9F` | 23 |  |
| `RES 4, (IX + d), B` | `DD CB d A0` | 23 |  |
| `RES 4, (IX + d), C` | `DD CB d A1` | 23 |  |
| `RES 4, (IX + d), D` | `DD CB d A2` | 23 |  |
| `RES 4, (IX + d), E` | `DD CB d A3` | 23 |  |
| `RES 4, (IX + d), H` | `DD CB d A4` | 23 |  |
| `RES 4, (IX + d), L` | `DD CB d A5` | 23 |  |
| `RES 4, (IX + d), A` | `DD CB d A7` | 23 |  |
| `RES 5, (IX + d), B` | `DD CB d A8` | 23 |  |
| `RES 5, (IX + d), C` | `DD CB d A9` | 23 |  |
| `RES 5, (IX + d), D` | `DD CB d AA` | 23 |  |
| `RES 5, (IX + d), E` | `DD CB d AB` | 23 |  |
| `RES 5, (IX + d), H` | `DD CB d AC` | 23 |  |
| `RES 5, (IX + d), L` | `DD CB d AD` | 23 |  |
| `RES 5, (IX + d), A` | `DD CB d AF` | 23 |  |
| `RES 6, (IX + d), B` | `DD CB d B0` | 23 |  |
| `RES 6, (IX + d), C` | `DD CB d B1` | 23 |  |
| `RES 6, (IX + d), D` | `DD CB d B2` | 23 |  |
| `RES 6, (IX + d), E` | `DD CB d B3` | 23 |  |
| `RES 6, (IX + d), H` | `DD CB d B4` | 23 |  |
| `RES 6, (IX + d), L` | `DD CB d B5` | 23 |  |
| `RES 6, (IX + d), A` | `DD CB d B7` | 23 |  |
| `RES 7, (IX + d), B` | `DD CB d B8` | 23 |  |
| `RES 7, (IX + d), C` | `DD CB d B9` | 23 |  |
| `RES 7, (IX + d), D` | `DD CB d BA` | 23 |  |
| `RES 7, (IX + d), E` | `DD CB d BB` | 23 |  |
| `RES 7, (IX + d), H` | `DD CB d BC` | 23 |  |
| `RES 7, (IX + d), L` | `DD CB d BD` | 23 |  |
| `RES 7, (IX + d), A` | `DD CB d BF` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
