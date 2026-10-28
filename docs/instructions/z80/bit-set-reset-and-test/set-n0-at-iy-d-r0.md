---
cpu: Z80
defined_by: Z80
form: SET N0, (IY + d), R0
mnemonic: SET b, (IY + d), r
group: Bit Set, Reset, and Test
description: "Sets bit {N0} of the byte at (IY + d), storing the result in both the byte at (IY + d) and {R0}."
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
  - encoding: FD CB d C0
    mnemonic: SET 0, (IY + d), B
    operands: { N0: 0, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d C1
    mnemonic: SET 0, (IY + d), C
    operands: { N0: 0, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d C2
    mnemonic: SET 0, (IY + d), D
    operands: { N0: 0, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d C3
    mnemonic: SET 0, (IY + d), E
    operands: { N0: 0, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d C4
    mnemonic: SET 0, (IY + d), H
    operands: { N0: 0, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d C5
    mnemonic: SET 0, (IY + d), L
    operands: { N0: 0, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d C7
    mnemonic: SET 0, (IY + d), A
    operands: { N0: 0, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d C8
    mnemonic: SET 1, (IY + d), B
    operands: { N0: 1, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d C9
    mnemonic: SET 1, (IY + d), C
    operands: { N0: 1, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d CA
    mnemonic: SET 1, (IY + d), D
    operands: { N0: 1, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d CB
    mnemonic: SET 1, (IY + d), E
    operands: { N0: 1, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d CC
    mnemonic: SET 1, (IY + d), H
    operands: { N0: 1, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d CD
    mnemonic: SET 1, (IY + d), L
    operands: { N0: 1, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d CF
    mnemonic: SET 1, (IY + d), A
    operands: { N0: 1, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d D0
    mnemonic: SET 2, (IY + d), B
    operands: { N0: 2, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d D1
    mnemonic: SET 2, (IY + d), C
    operands: { N0: 2, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d D2
    mnemonic: SET 2, (IY + d), D
    operands: { N0: 2, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d D3
    mnemonic: SET 2, (IY + d), E
    operands: { N0: 2, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d D4
    mnemonic: SET 2, (IY + d), H
    operands: { N0: 2, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d D5
    mnemonic: SET 2, (IY + d), L
    operands: { N0: 2, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d D7
    mnemonic: SET 2, (IY + d), A
    operands: { N0: 2, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d D8
    mnemonic: SET 3, (IY + d), B
    operands: { N0: 3, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d D9
    mnemonic: SET 3, (IY + d), C
    operands: { N0: 3, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d DA
    mnemonic: SET 3, (IY + d), D
    operands: { N0: 3, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d DB
    mnemonic: SET 3, (IY + d), E
    operands: { N0: 3, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d DC
    mnemonic: SET 3, (IY + d), H
    operands: { N0: 3, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d DD
    mnemonic: SET 3, (IY + d), L
    operands: { N0: 3, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d DF
    mnemonic: SET 3, (IY + d), A
    operands: { N0: 3, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d E0
    mnemonic: SET 4, (IY + d), B
    operands: { N0: 4, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d E1
    mnemonic: SET 4, (IY + d), C
    operands: { N0: 4, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d E2
    mnemonic: SET 4, (IY + d), D
    operands: { N0: 4, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d E3
    mnemonic: SET 4, (IY + d), E
    operands: { N0: 4, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d E4
    mnemonic: SET 4, (IY + d), H
    operands: { N0: 4, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d E5
    mnemonic: SET 4, (IY + d), L
    operands: { N0: 4, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d E7
    mnemonic: SET 4, (IY + d), A
    operands: { N0: 4, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d E8
    mnemonic: SET 5, (IY + d), B
    operands: { N0: 5, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d E9
    mnemonic: SET 5, (IY + d), C
    operands: { N0: 5, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d EA
    mnemonic: SET 5, (IY + d), D
    operands: { N0: 5, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d EB
    mnemonic: SET 5, (IY + d), E
    operands: { N0: 5, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d EC
    mnemonic: SET 5, (IY + d), H
    operands: { N0: 5, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d ED
    mnemonic: SET 5, (IY + d), L
    operands: { N0: 5, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d EF
    mnemonic: SET 5, (IY + d), A
    operands: { N0: 5, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d F0
    mnemonic: SET 6, (IY + d), B
    operands: { N0: 6, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d F1
    mnemonic: SET 6, (IY + d), C
    operands: { N0: 6, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d F2
    mnemonic: SET 6, (IY + d), D
    operands: { N0: 6, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d F3
    mnemonic: SET 6, (IY + d), E
    operands: { N0: 6, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d F4
    mnemonic: SET 6, (IY + d), H
    operands: { N0: 6, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d F5
    mnemonic: SET 6, (IY + d), L
    operands: { N0: 6, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d F7
    mnemonic: SET 6, (IY + d), A
    operands: { N0: 6, R0: A }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d F8
    mnemonic: SET 7, (IY + d), B
    operands: { N0: 7, R0: B }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d F9
    mnemonic: SET 7, (IY + d), C
    operands: { N0: 7, R0: C }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d FA
    mnemonic: SET 7, (IY + d), D
    operands: { N0: 7, R0: D }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d FB
    mnemonic: SET 7, (IY + d), E
    operands: { N0: 7, R0: E }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d FC
    mnemonic: SET 7, (IY + d), H
    operands: { N0: 7, R0: H }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d FD
    mnemonic: SET 7, (IY + d), L
    operands: { N0: 7, R0: L }
    tstates: [23]
    undocumented: true
  - encoding: FD CB d FF
    mnemonic: SET 7, (IY + d), A
    operands: { N0: 7, R0: A }
    tstates: [23]
    undocumented: true
references:
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
  - name: The Undocumented Z80 Documented
    url: "http://www.z80.info/zip/z80-documented.pdf"
---

# SET b, (IY + d), r

Sets bit b of the byte at (IY + d), storing the result in both the byte at (IY + d) and r.

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
| `SET 0, (IY + d), B` | `FD CB d C0` | 23 |  |
| `SET 0, (IY + d), C` | `FD CB d C1` | 23 |  |
| `SET 0, (IY + d), D` | `FD CB d C2` | 23 |  |
| `SET 0, (IY + d), E` | `FD CB d C3` | 23 |  |
| `SET 0, (IY + d), H` | `FD CB d C4` | 23 |  |
| `SET 0, (IY + d), L` | `FD CB d C5` | 23 |  |
| `SET 0, (IY + d), A` | `FD CB d C7` | 23 |  |
| `SET 1, (IY + d), B` | `FD CB d C8` | 23 |  |
| `SET 1, (IY + d), C` | `FD CB d C9` | 23 |  |
| `SET 1, (IY + d), D` | `FD CB d CA` | 23 |  |
| `SET 1, (IY + d), E` | `FD CB d CB` | 23 |  |
| `SET 1, (IY + d), H` | `FD CB d CC` | 23 |  |
| `SET 1, (IY + d), L` | `FD CB d CD` | 23 |  |
| `SET 1, (IY + d), A` | `FD CB d CF` | 23 |  |
| `SET 2, (IY + d), B` | `FD CB d D0` | 23 |  |
| `SET 2, (IY + d), C` | `FD CB d D1` | 23 |  |
| `SET 2, (IY + d), D` | `FD CB d D2` | 23 |  |
| `SET 2, (IY + d), E` | `FD CB d D3` | 23 |  |
| `SET 2, (IY + d), H` | `FD CB d D4` | 23 |  |
| `SET 2, (IY + d), L` | `FD CB d D5` | 23 |  |
| `SET 2, (IY + d), A` | `FD CB d D7` | 23 |  |
| `SET 3, (IY + d), B` | `FD CB d D8` | 23 |  |
| `SET 3, (IY + d), C` | `FD CB d D9` | 23 |  |
| `SET 3, (IY + d), D` | `FD CB d DA` | 23 |  |
| `SET 3, (IY + d), E` | `FD CB d DB` | 23 |  |
| `SET 3, (IY + d), H` | `FD CB d DC` | 23 |  |
| `SET 3, (IY + d), L` | `FD CB d DD` | 23 |  |
| `SET 3, (IY + d), A` | `FD CB d DF` | 23 |  |
| `SET 4, (IY + d), B` | `FD CB d E0` | 23 |  |
| `SET 4, (IY + d), C` | `FD CB d E1` | 23 |  |
| `SET 4, (IY + d), D` | `FD CB d E2` | 23 |  |
| `SET 4, (IY + d), E` | `FD CB d E3` | 23 |  |
| `SET 4, (IY + d), H` | `FD CB d E4` | 23 |  |
| `SET 4, (IY + d), L` | `FD CB d E5` | 23 |  |
| `SET 4, (IY + d), A` | `FD CB d E7` | 23 |  |
| `SET 5, (IY + d), B` | `FD CB d E8` | 23 |  |
| `SET 5, (IY + d), C` | `FD CB d E9` | 23 |  |
| `SET 5, (IY + d), D` | `FD CB d EA` | 23 |  |
| `SET 5, (IY + d), E` | `FD CB d EB` | 23 |  |
| `SET 5, (IY + d), H` | `FD CB d EC` | 23 |  |
| `SET 5, (IY + d), L` | `FD CB d ED` | 23 |  |
| `SET 5, (IY + d), A` | `FD CB d EF` | 23 |  |
| `SET 6, (IY + d), B` | `FD CB d F0` | 23 |  |
| `SET 6, (IY + d), C` | `FD CB d F1` | 23 |  |
| `SET 6, (IY + d), D` | `FD CB d F2` | 23 |  |
| `SET 6, (IY + d), E` | `FD CB d F3` | 23 |  |
| `SET 6, (IY + d), H` | `FD CB d F4` | 23 |  |
| `SET 6, (IY + d), L` | `FD CB d F5` | 23 |  |
| `SET 6, (IY + d), A` | `FD CB d F7` | 23 |  |
| `SET 7, (IY + d), B` | `FD CB d F8` | 23 |  |
| `SET 7, (IY + d), C` | `FD CB d F9` | 23 |  |
| `SET 7, (IY + d), D` | `FD CB d FA` | 23 |  |
| `SET 7, (IY + d), E` | `FD CB d FB` | 23 |  |
| `SET 7, (IY + d), H` | `FD CB d FC` | 23 |  |
| `SET 7, (IY + d), L` | `FD CB d FD` | 23 |  |
| `SET 7, (IY + d), A` | `FD CB d FF` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
