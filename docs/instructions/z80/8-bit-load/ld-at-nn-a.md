---
cpu: Z80
defined_by: Z80
form: LD (nn), A
mnemonic: LD (nn), A
group: 8-Bit Load
description: Stores A at address nn.
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
  - encoding: 32 nn
    mnemonic: LD (nn), A
    tstates: [13]
    undocumented: false
  - encoding: DD 32 nn
    mnemonic: LD (nn), A
    tstates: [17]
    undocumented: true
  - encoding: FD 32 nn
    mnemonic: LD (nn), A
    tstates: [17]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD (nn), A

Stores A at address nn.

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
| `LD (nn), A` | `32 nn` | 13 |  |
| `LD (nn), A` | `DD 32 nn` | 17 | Undocumented. |
| `LD (nn), A` | `FD 32 nn` | 17 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
