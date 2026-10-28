---
cpu: Z80N
defined_by: Z80
form: OUT (C), R0
mnemonic: OUT (C), r
group: Input and Output
description: "Writes {R0} to the port addressed by BC."
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
  - encoding: ED 41
    mnemonic: OUT (C), B
    operands: { R0: B }
    tstates: [12]
    undocumented: false
  - encoding: ED 49
    mnemonic: OUT (C), C
    operands: { R0: C }
    tstates: [12]
    undocumented: false
  - encoding: ED 51
    mnemonic: OUT (C), D
    operands: { R0: D }
    tstates: [12]
    undocumented: false
  - encoding: ED 59
    mnemonic: OUT (C), E
    operands: { R0: E }
    tstates: [12]
    undocumented: false
  - encoding: ED 61
    mnemonic: OUT (C), H
    operands: { R0: H }
    tstates: [12]
    undocumented: false
  - encoding: ED 69
    mnemonic: OUT (C), L
    operands: { R0: L }
    tstates: [12]
    undocumented: false
  - encoding: ED 79
    mnemonic: OUT (C), A
    operands: { R0: A }
    tstates: [12]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# OUT (C), r

Writes r to the port addressed by BC.

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
| `OUT (C), B` | `ED 41` | 12 |  |
| `OUT (C), C` | `ED 49` | 12 |  |
| `OUT (C), D` | `ED 51` | 12 |  |
| `OUT (C), E` | `ED 59` | 12 |  |
| `OUT (C), H` | `ED 61` | 12 |  |
| `OUT (C), L` | `ED 69` | 12 |  |
| `OUT (C), A` | `ED 79` | 12 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
