---
cpu: Z80
defined_by: Z80
form: IN A, (n)
mnemonic: IN A, (n)
group: Input and Output
description: Reads a byte from port n into A, with A on the high half of the address bus.
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
  - encoding: DB n
    mnemonic: IN A, (n)
    tstates: [11]
    undocumented: false
  - encoding: DD DB n
    mnemonic: IN A, (n)
    tstates: [15]
    undocumented: true
  - encoding: FD DB n
    mnemonic: IN A, (n)
    tstates: [15]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# IN A, (n)

Reads a byte from port n into A, with A on the high half of the address bus.

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
| `IN A, (n)` | `DB n` | 11 |  |
| `IN A, (n)` | `DD DB n` | 15 | Undocumented. |
| `IN A, (n)` | `FD DB n` | 15 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
