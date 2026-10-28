---
cpu: Z80N
defined_by: Z80
form: LD R0, (RP0 + d)
mnemonic: LD r, (rr + d)
group: 8-Bit Load
description: "Loads the byte at ({RP0} + d) into {R0}."
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
  - encoding: DD 46 d
    mnemonic: LD B, (IX + d)
    operands: { R0: B, RP0: IX }
    tstates: [19]
    undocumented: false
  - encoding: DD 4E d
    mnemonic: LD C, (IX + d)
    operands: { R0: C, RP0: IX }
    tstates: [19]
    undocumented: false
  - encoding: DD 56 d
    mnemonic: LD D, (IX + d)
    operands: { R0: D, RP0: IX }
    tstates: [19]
    undocumented: false
  - encoding: DD 5E d
    mnemonic: LD E, (IX + d)
    operands: { R0: E, RP0: IX }
    tstates: [19]
    undocumented: false
  - encoding: DD 66 d
    mnemonic: LD H, (IX + d)
    operands: { R0: H, RP0: IX }
    tstates: [19]
    undocumented: false
  - encoding: DD 6E d
    mnemonic: LD L, (IX + d)
    operands: { R0: L, RP0: IX }
    tstates: [19]
    undocumented: false
  - encoding: DD 7E d
    mnemonic: LD A, (IX + d)
    operands: { R0: A, RP0: IX }
    tstates: [19]
    undocumented: false
  - encoding: FD 46 d
    mnemonic: LD B, (IY + d)
    operands: { R0: B, RP0: IY }
    tstates: [19]
    undocumented: false
  - encoding: FD 4E d
    mnemonic: LD C, (IY + d)
    operands: { R0: C, RP0: IY }
    tstates: [19]
    undocumented: false
  - encoding: FD 56 d
    mnemonic: LD D, (IY + d)
    operands: { R0: D, RP0: IY }
    tstates: [19]
    undocumented: false
  - encoding: FD 5E d
    mnemonic: LD E, (IY + d)
    operands: { R0: E, RP0: IY }
    tstates: [19]
    undocumented: false
  - encoding: FD 66 d
    mnemonic: LD H, (IY + d)
    operands: { R0: H, RP0: IY }
    tstates: [19]
    undocumented: false
  - encoding: FD 6E d
    mnemonic: LD L, (IY + d)
    operands: { R0: L, RP0: IY }
    tstates: [19]
    undocumented: false
  - encoding: FD 7E d
    mnemonic: LD A, (IY + d)
    operands: { R0: A, RP0: IY }
    tstates: [19]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD r, (rr + d)

Loads the byte at (rr + d) into r.

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
| `LD B, (IX + d)` | `DD 46 d` | 19 |  |
| `LD C, (IX + d)` | `DD 4E d` | 19 |  |
| `LD D, (IX + d)` | `DD 56 d` | 19 |  |
| `LD E, (IX + d)` | `DD 5E d` | 19 |  |
| `LD H, (IX + d)` | `DD 66 d` | 19 |  |
| `LD L, (IX + d)` | `DD 6E d` | 19 |  |
| `LD A, (IX + d)` | `DD 7E d` | 19 |  |
| `LD B, (IY + d)` | `FD 46 d` | 19 |  |
| `LD C, (IY + d)` | `FD 4E d` | 19 |  |
| `LD D, (IY + d)` | `FD 56 d` | 19 |  |
| `LD E, (IY + d)` | `FD 5E d` | 19 |  |
| `LD H, (IY + d)` | `FD 66 d` | 19 |  |
| `LD L, (IY + d)` | `FD 6E d` | 19 |  |
| `LD A, (IY + d)` | `FD 7E d` | 19 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
