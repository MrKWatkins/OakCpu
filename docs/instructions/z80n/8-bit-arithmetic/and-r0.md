---
cpu: Z80N
defined_by: Z80
form: AND R0
mnemonic: AND r
group: 8-Bit Arithmetic
description: "Sets A to the bitwise AND of A and {R0}."
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of A.
  - flag: Z
    effect: changed
    description: Set if A is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of A.
  - flag: H
    effect: set
    description: Set.
  - flag: X
    effect: changed
    description: Copied from bit 3 of A.
  - flag: PV
    effect: changed
    description: Set if A has even parity, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: reset
    description: Reset.
opcodes:
  - encoding: A0
    mnemonic: AND B
    operands: { R0: B }
    tstates: [4]
    undocumented: false
  - encoding: A1
    mnemonic: AND C
    operands: { R0: C }
    tstates: [4]
    undocumented: false
  - encoding: A2
    mnemonic: AND D
    operands: { R0: D }
    tstates: [4]
    undocumented: false
  - encoding: A3
    mnemonic: AND E
    operands: { R0: E }
    tstates: [4]
    undocumented: false
  - encoding: A4
    mnemonic: AND H
    operands: { R0: H }
    tstates: [4]
    undocumented: false
  - encoding: A5
    mnemonic: AND L
    operands: { R0: L }
    tstates: [4]
    undocumented: false
  - encoding: A7
    mnemonic: AND A
    operands: { R0: A }
    tstates: [4]
    undocumented: false
  - encoding: DD A0
    mnemonic: AND B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: DD A1
    mnemonic: AND C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: DD A2
    mnemonic: AND D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: DD A3
    mnemonic: AND E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: DD A4
    mnemonic: AND IXH
    operands: { R0: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD A5
    mnemonic: AND IXL
    operands: { R0: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD A7
    mnemonic: AND A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
  - encoding: FD A0
    mnemonic: AND B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: FD A1
    mnemonic: AND C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: FD A2
    mnemonic: AND D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: FD A3
    mnemonic: AND E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: FD A4
    mnemonic: AND IYH
    operands: { R0: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD A5
    mnemonic: AND IYL
    operands: { R0: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD A7
    mnemonic: AND A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# AND r

Sets A to the bitwise AND of A and r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of A. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of A. |
| H | Set. |
| X | Copied from bit 3 of A. |
| PV | Set if A has even parity, otherwise reset. |
| N | Reset. |
| C | Reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `AND B` | `A0` | 4 |  |
| `AND C` | `A1` | 4 |  |
| `AND D` | `A2` | 4 |  |
| `AND E` | `A3` | 4 |  |
| `AND H` | `A4` | 4 |  |
| `AND L` | `A5` | 4 |  |
| `AND A` | `A7` | 4 |  |
| `AND B` | `DD A0` | 8 | Undocumented. |
| `AND C` | `DD A1` | 8 | Undocumented. |
| `AND D` | `DD A2` | 8 | Undocumented. |
| `AND E` | `DD A3` | 8 | Undocumented. |
| `AND IXH` | `DD A4` | 8 | Undocumented. |
| `AND IXL` | `DD A5` | 8 | Undocumented. |
| `AND A` | `DD A7` | 8 | Undocumented. |
| `AND B` | `FD A0` | 8 | Undocumented. |
| `AND C` | `FD A1` | 8 | Undocumented. |
| `AND D` | `FD A2` | 8 | Undocumented. |
| `AND E` | `FD A3` | 8 | Undocumented. |
| `AND IYH` | `FD A4` | 8 | Undocumented. |
| `AND IYL` | `FD A5` | 8 | Undocumented. |
| `AND A` | `FD A7` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
