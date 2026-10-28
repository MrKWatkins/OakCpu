---
cpu: Z80N
defined_by: Z80
form: LD A, (nn)
mnemonic: LD A, (nn)
group: 8-Bit Load
description: Loads the byte at address nn into A.
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
  - encoding: 3A nn
    mnemonic: LD A, (nn)
    tstates: [13]
    undocumented: false
  - encoding: DD 3A nn
    mnemonic: LD A, (nn)
    tstates: [17]
    undocumented: true
  - encoding: FD 3A nn
    mnemonic: LD A, (nn)
    tstates: [17]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD A, (nn)

Loads the byte at address nn into A.

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
| `LD A, (nn)` | `3A nn` | 13 |  |
| `LD A, (nn)` | `DD 3A nn` | 17 | Undocumented. |
| `LD A, (nn)` | `FD 3A nn` | 17 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
