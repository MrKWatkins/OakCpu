---
cpu: Z80
defined_by: Z80
form: LD (RP0), A
mnemonic: LD (rr), A
group: 8-Bit Load
description: "Stores A at ({RP0})."
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
  - encoding: 02
    mnemonic: LD (BC), A
    operands: { RP0: BC }
    tstates: [7]
    undocumented: false
  - encoding: 12
    mnemonic: LD (DE), A
    operands: { RP0: DE }
    tstates: [7]
    undocumented: false
  - encoding: DD 02
    mnemonic: LD (BC), A
    operands: { RP0: BC }
    tstates: [11]
    undocumented: true
  - encoding: DD 12
    mnemonic: LD (DE), A
    operands: { RP0: DE }
    tstates: [11]
    undocumented: true
  - encoding: FD 02
    mnemonic: LD (BC), A
    operands: { RP0: BC }
    tstates: [11]
    undocumented: true
  - encoding: FD 12
    mnemonic: LD (DE), A
    operands: { RP0: DE }
    tstates: [11]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD (rr), A

Stores A at (rr).

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
| `LD (BC), A` | `02` | 7 |  |
| `LD (DE), A` | `12` | 7 |  |
| `LD (BC), A` | `DD 02` | 11 | Undocumented. |
| `LD (DE), A` | `DD 12` | 11 | Undocumented. |
| `LD (BC), A` | `FD 02` | 11 | Undocumented. |
| `LD (DE), A` | `FD 12` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
