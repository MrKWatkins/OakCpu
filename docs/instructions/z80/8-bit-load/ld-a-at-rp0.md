---
cpu: Z80
defined_by: Z80
form: LD A, (RP0)
mnemonic: LD A, (rr)
group: 8-Bit Load
description: "Loads the byte at ({RP0}) into A."
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
  - encoding: 0A
    mnemonic: LD A, (BC)
    operands: { RP0: BC }
    tstates: [7]
    undocumented: false
  - encoding: 1A
    mnemonic: LD A, (DE)
    operands: { RP0: DE }
    tstates: [7]
    undocumented: false
  - encoding: DD 0A
    mnemonic: LD A, (BC)
    operands: { RP0: BC }
    tstates: [11]
    undocumented: true
  - encoding: DD 1A
    mnemonic: LD A, (DE)
    operands: { RP0: DE }
    tstates: [11]
    undocumented: true
  - encoding: FD 0A
    mnemonic: LD A, (BC)
    operands: { RP0: BC }
    tstates: [11]
    undocumented: true
  - encoding: FD 1A
    mnemonic: LD A, (DE)
    operands: { RP0: DE }
    tstates: [11]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD A, (rr)

Loads the byte at (rr) into A.

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
| `LD A, (BC)` | `0A` | 7 |  |
| `LD A, (DE)` | `1A` | 7 |  |
| `LD A, (BC)` | `DD 0A` | 11 | Undocumented. |
| `LD A, (DE)` | `DD 1A` | 11 | Undocumented. |
| `LD A, (BC)` | `FD 0A` | 11 | Undocumented. |
| `LD A, (DE)` | `FD 1A` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
