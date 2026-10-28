---
cpu: Z80
defined_by: Z80
form: LD (RP0 + d), R0
mnemonic: LD (rr + d), r
group: 8-Bit Load
description: "Stores {R0} at ({RP0} + d)."
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
  - encoding: DD 70 d
    mnemonic: LD (IX + d), B
    operands: { RP0: IX, R0: B }
    tstates: [19]
    undocumented: false
  - encoding: DD 71 d
    mnemonic: LD (IX + d), C
    operands: { RP0: IX, R0: C }
    tstates: [19]
    undocumented: false
  - encoding: DD 72 d
    mnemonic: LD (IX + d), D
    operands: { RP0: IX, R0: D }
    tstates: [19]
    undocumented: false
  - encoding: DD 73 d
    mnemonic: LD (IX + d), E
    operands: { RP0: IX, R0: E }
    tstates: [19]
    undocumented: false
  - encoding: DD 74 d
    mnemonic: LD (IX + d), H
    operands: { RP0: IX, R0: H }
    tstates: [19]
    undocumented: false
  - encoding: DD 75 d
    mnemonic: LD (IX + d), L
    operands: { RP0: IX, R0: L }
    tstates: [19]
    undocumented: false
  - encoding: DD 77 d
    mnemonic: LD (IX + d), A
    operands: { RP0: IX, R0: A }
    tstates: [19]
    undocumented: false
  - encoding: FD 70 d
    mnemonic: LD (IY + d), B
    operands: { RP0: IY, R0: B }
    tstates: [19]
    undocumented: false
  - encoding: FD 71 d
    mnemonic: LD (IY + d), C
    operands: { RP0: IY, R0: C }
    tstates: [19]
    undocumented: false
  - encoding: FD 72 d
    mnemonic: LD (IY + d), D
    operands: { RP0: IY, R0: D }
    tstates: [19]
    undocumented: false
  - encoding: FD 73 d
    mnemonic: LD (IY + d), E
    operands: { RP0: IY, R0: E }
    tstates: [19]
    undocumented: false
  - encoding: FD 74 d
    mnemonic: LD (IY + d), H
    operands: { RP0: IY, R0: H }
    tstates: [19]
    undocumented: false
  - encoding: FD 75 d
    mnemonic: LD (IY + d), L
    operands: { RP0: IY, R0: L }
    tstates: [19]
    undocumented: false
  - encoding: FD 77 d
    mnemonic: LD (IY + d), A
    operands: { RP0: IY, R0: A }
    tstates: [19]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD (rr + d), r

Stores r at (rr + d).

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
| `LD (IX + d), B` | `DD 70 d` | 19 |  |
| `LD (IX + d), C` | `DD 71 d` | 19 |  |
| `LD (IX + d), D` | `DD 72 d` | 19 |  |
| `LD (IX + d), E` | `DD 73 d` | 19 |  |
| `LD (IX + d), H` | `DD 74 d` | 19 |  |
| `LD (IX + d), L` | `DD 75 d` | 19 |  |
| `LD (IX + d), A` | `DD 77 d` | 19 |  |
| `LD (IY + d), B` | `FD 70 d` | 19 |  |
| `LD (IY + d), C` | `FD 71 d` | 19 |  |
| `LD (IY + d), D` | `FD 72 d` | 19 |  |
| `LD (IY + d), E` | `FD 73 d` | 19 |  |
| `LD (IY + d), H` | `FD 74 d` | 19 |  |
| `LD (IY + d), L` | `FD 75 d` | 19 |  |
| `LD (IY + d), A` | `FD 77 d` | 19 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
