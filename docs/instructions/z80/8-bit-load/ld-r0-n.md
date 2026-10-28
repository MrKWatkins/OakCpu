---
cpu: Z80
defined_by: Z80
form: LD R0, n
mnemonic: LD r, n
group: 8-Bit Load
description: "Loads n into {R0}."
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
  - encoding: 06 n
    mnemonic: LD B, n
    operands: { R0: B }
    tstates: [7]
    undocumented: false
  - encoding: 0E n
    mnemonic: LD C, n
    operands: { R0: C }
    tstates: [7]
    undocumented: false
  - encoding: 16 n
    mnemonic: LD D, n
    operands: { R0: D }
    tstates: [7]
    undocumented: false
  - encoding: 1E n
    mnemonic: LD E, n
    operands: { R0: E }
    tstates: [7]
    undocumented: false
  - encoding: 26 n
    mnemonic: LD H, n
    operands: { R0: H }
    tstates: [7]
    undocumented: false
  - encoding: 2E n
    mnemonic: LD L, n
    operands: { R0: L }
    tstates: [7]
    undocumented: false
  - encoding: 3E n
    mnemonic: LD A, n
    operands: { R0: A }
    tstates: [7]
    undocumented: false
  - encoding: DD 06 n
    mnemonic: LD B, n
    operands: { R0: B }
    tstates: [11]
    undocumented: true
  - encoding: DD 0E n
    mnemonic: LD C, n
    operands: { R0: C }
    tstates: [11]
    undocumented: true
  - encoding: DD 16 n
    mnemonic: LD D, n
    operands: { R0: D }
    tstates: [11]
    undocumented: true
  - encoding: DD 1E n
    mnemonic: LD E, n
    operands: { R0: E }
    tstates: [11]
    undocumented: true
  - encoding: DD 26 n
    mnemonic: LD IXH, n
    operands: { R0: IXH }
    tstates: [11]
    undocumented: true
  - encoding: DD 2E n
    mnemonic: LD IXL, n
    operands: { R0: IXL }
    tstates: [11]
    undocumented: true
  - encoding: DD 3E n
    mnemonic: LD A, n
    operands: { R0: A }
    tstates: [11]
    undocumented: true
  - encoding: FD 06 n
    mnemonic: LD B, n
    operands: { R0: B }
    tstates: [11]
    undocumented: true
  - encoding: FD 0E n
    mnemonic: LD C, n
    operands: { R0: C }
    tstates: [11]
    undocumented: true
  - encoding: FD 16 n
    mnemonic: LD D, n
    operands: { R0: D }
    tstates: [11]
    undocumented: true
  - encoding: FD 1E n
    mnemonic: LD E, n
    operands: { R0: E }
    tstates: [11]
    undocumented: true
  - encoding: FD 26 n
    mnemonic: LD IYH, n
    operands: { R0: IYH }
    tstates: [11]
    undocumented: true
  - encoding: FD 2E n
    mnemonic: LD IYL, n
    operands: { R0: IYL }
    tstates: [11]
    undocumented: true
  - encoding: FD 3E n
    mnemonic: LD A, n
    operands: { R0: A }
    tstates: [11]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD r, n

Loads n into r.

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
| `LD B, n` | `06 n` | 7 |  |
| `LD C, n` | `0E n` | 7 |  |
| `LD D, n` | `16 n` | 7 |  |
| `LD E, n` | `1E n` | 7 |  |
| `LD H, n` | `26 n` | 7 |  |
| `LD L, n` | `2E n` | 7 |  |
| `LD A, n` | `3E n` | 7 |  |
| `LD B, n` | `DD 06 n` | 11 | Undocumented. |
| `LD C, n` | `DD 0E n` | 11 | Undocumented. |
| `LD D, n` | `DD 16 n` | 11 | Undocumented. |
| `LD E, n` | `DD 1E n` | 11 | Undocumented. |
| `LD IXH, n` | `DD 26 n` | 11 | Undocumented. |
| `LD IXL, n` | `DD 2E n` | 11 | Undocumented. |
| `LD A, n` | `DD 3E n` | 11 | Undocumented. |
| `LD B, n` | `FD 06 n` | 11 | Undocumented. |
| `LD C, n` | `FD 0E n` | 11 | Undocumented. |
| `LD D, n` | `FD 16 n` | 11 | Undocumented. |
| `LD E, n` | `FD 1E n` | 11 | Undocumented. |
| `LD IYH, n` | `FD 26 n` | 11 | Undocumented. |
| `LD IYL, n` | `FD 2E n` | 11 | Undocumented. |
| `LD A, n` | `FD 3E n` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
