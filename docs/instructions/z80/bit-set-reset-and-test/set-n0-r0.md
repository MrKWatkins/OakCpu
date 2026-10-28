---
cpu: Z80
defined_by: Z80
form: SET N0, R0
mnemonic: SET b, r
group: Bit Set, Reset, and Test
description: "Sets bit {N0} of {R0}."
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
  - encoding: CB C0
    mnemonic: SET 0, B
    operands: { N0: 0, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB C1
    mnemonic: SET 0, C
    operands: { N0: 0, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB C2
    mnemonic: SET 0, D
    operands: { N0: 0, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB C3
    mnemonic: SET 0, E
    operands: { N0: 0, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB C4
    mnemonic: SET 0, H
    operands: { N0: 0, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB C5
    mnemonic: SET 0, L
    operands: { N0: 0, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB C7
    mnemonic: SET 0, A
    operands: { N0: 0, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB C8
    mnemonic: SET 1, B
    operands: { N0: 1, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB C9
    mnemonic: SET 1, C
    operands: { N0: 1, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB CA
    mnemonic: SET 1, D
    operands: { N0: 1, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB CB
    mnemonic: SET 1, E
    operands: { N0: 1, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB CC
    mnemonic: SET 1, H
    operands: { N0: 1, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB CD
    mnemonic: SET 1, L
    operands: { N0: 1, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB CF
    mnemonic: SET 1, A
    operands: { N0: 1, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB D0
    mnemonic: SET 2, B
    operands: { N0: 2, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB D1
    mnemonic: SET 2, C
    operands: { N0: 2, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB D2
    mnemonic: SET 2, D
    operands: { N0: 2, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB D3
    mnemonic: SET 2, E
    operands: { N0: 2, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB D4
    mnemonic: SET 2, H
    operands: { N0: 2, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB D5
    mnemonic: SET 2, L
    operands: { N0: 2, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB D7
    mnemonic: SET 2, A
    operands: { N0: 2, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB D8
    mnemonic: SET 3, B
    operands: { N0: 3, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB D9
    mnemonic: SET 3, C
    operands: { N0: 3, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB DA
    mnemonic: SET 3, D
    operands: { N0: 3, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB DB
    mnemonic: SET 3, E
    operands: { N0: 3, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB DC
    mnemonic: SET 3, H
    operands: { N0: 3, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB DD
    mnemonic: SET 3, L
    operands: { N0: 3, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB DF
    mnemonic: SET 3, A
    operands: { N0: 3, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB E0
    mnemonic: SET 4, B
    operands: { N0: 4, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB E1
    mnemonic: SET 4, C
    operands: { N0: 4, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB E2
    mnemonic: SET 4, D
    operands: { N0: 4, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB E3
    mnemonic: SET 4, E
    operands: { N0: 4, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB E4
    mnemonic: SET 4, H
    operands: { N0: 4, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB E5
    mnemonic: SET 4, L
    operands: { N0: 4, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB E7
    mnemonic: SET 4, A
    operands: { N0: 4, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB E8
    mnemonic: SET 5, B
    operands: { N0: 5, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB E9
    mnemonic: SET 5, C
    operands: { N0: 5, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB EA
    mnemonic: SET 5, D
    operands: { N0: 5, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB EB
    mnemonic: SET 5, E
    operands: { N0: 5, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB EC
    mnemonic: SET 5, H
    operands: { N0: 5, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB ED
    mnemonic: SET 5, L
    operands: { N0: 5, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB EF
    mnemonic: SET 5, A
    operands: { N0: 5, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB F0
    mnemonic: SET 6, B
    operands: { N0: 6, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB F1
    mnemonic: SET 6, C
    operands: { N0: 6, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB F2
    mnemonic: SET 6, D
    operands: { N0: 6, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB F3
    mnemonic: SET 6, E
    operands: { N0: 6, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB F4
    mnemonic: SET 6, H
    operands: { N0: 6, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB F5
    mnemonic: SET 6, L
    operands: { N0: 6, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB F7
    mnemonic: SET 6, A
    operands: { N0: 6, R0: A }
    tstates: [8]
    undocumented: false
  - encoding: CB F8
    mnemonic: SET 7, B
    operands: { N0: 7, R0: B }
    tstates: [8]
    undocumented: false
  - encoding: CB F9
    mnemonic: SET 7, C
    operands: { N0: 7, R0: C }
    tstates: [8]
    undocumented: false
  - encoding: CB FA
    mnemonic: SET 7, D
    operands: { N0: 7, R0: D }
    tstates: [8]
    undocumented: false
  - encoding: CB FB
    mnemonic: SET 7, E
    operands: { N0: 7, R0: E }
    tstates: [8]
    undocumented: false
  - encoding: CB FC
    mnemonic: SET 7, H
    operands: { N0: 7, R0: H }
    tstates: [8]
    undocumented: false
  - encoding: CB FD
    mnemonic: SET 7, L
    operands: { N0: 7, R0: L }
    tstates: [8]
    undocumented: false
  - encoding: CB FF
    mnemonic: SET 7, A
    operands: { N0: 7, R0: A }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SET b, r

Sets bit b of r.

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
| `SET 0, B` | `CB C0` | 8 |  |
| `SET 0, C` | `CB C1` | 8 |  |
| `SET 0, D` | `CB C2` | 8 |  |
| `SET 0, E` | `CB C3` | 8 |  |
| `SET 0, H` | `CB C4` | 8 |  |
| `SET 0, L` | `CB C5` | 8 |  |
| `SET 0, A` | `CB C7` | 8 |  |
| `SET 1, B` | `CB C8` | 8 |  |
| `SET 1, C` | `CB C9` | 8 |  |
| `SET 1, D` | `CB CA` | 8 |  |
| `SET 1, E` | `CB CB` | 8 |  |
| `SET 1, H` | `CB CC` | 8 |  |
| `SET 1, L` | `CB CD` | 8 |  |
| `SET 1, A` | `CB CF` | 8 |  |
| `SET 2, B` | `CB D0` | 8 |  |
| `SET 2, C` | `CB D1` | 8 |  |
| `SET 2, D` | `CB D2` | 8 |  |
| `SET 2, E` | `CB D3` | 8 |  |
| `SET 2, H` | `CB D4` | 8 |  |
| `SET 2, L` | `CB D5` | 8 |  |
| `SET 2, A` | `CB D7` | 8 |  |
| `SET 3, B` | `CB D8` | 8 |  |
| `SET 3, C` | `CB D9` | 8 |  |
| `SET 3, D` | `CB DA` | 8 |  |
| `SET 3, E` | `CB DB` | 8 |  |
| `SET 3, H` | `CB DC` | 8 |  |
| `SET 3, L` | `CB DD` | 8 |  |
| `SET 3, A` | `CB DF` | 8 |  |
| `SET 4, B` | `CB E0` | 8 |  |
| `SET 4, C` | `CB E1` | 8 |  |
| `SET 4, D` | `CB E2` | 8 |  |
| `SET 4, E` | `CB E3` | 8 |  |
| `SET 4, H` | `CB E4` | 8 |  |
| `SET 4, L` | `CB E5` | 8 |  |
| `SET 4, A` | `CB E7` | 8 |  |
| `SET 5, B` | `CB E8` | 8 |  |
| `SET 5, C` | `CB E9` | 8 |  |
| `SET 5, D` | `CB EA` | 8 |  |
| `SET 5, E` | `CB EB` | 8 |  |
| `SET 5, H` | `CB EC` | 8 |  |
| `SET 5, L` | `CB ED` | 8 |  |
| `SET 5, A` | `CB EF` | 8 |  |
| `SET 6, B` | `CB F0` | 8 |  |
| `SET 6, C` | `CB F1` | 8 |  |
| `SET 6, D` | `CB F2` | 8 |  |
| `SET 6, E` | `CB F3` | 8 |  |
| `SET 6, H` | `CB F4` | 8 |  |
| `SET 6, L` | `CB F5` | 8 |  |
| `SET 6, A` | `CB F7` | 8 |  |
| `SET 7, B` | `CB F8` | 8 |  |
| `SET 7, C` | `CB F9` | 8 |  |
| `SET 7, D` | `CB FA` | 8 |  |
| `SET 7, E` | `CB FB` | 8 |  |
| `SET 7, H` | `CB FC` | 8 |  |
| `SET 7, L` | `CB FD` | 8 |  |
| `SET 7, A` | `CB FF` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
