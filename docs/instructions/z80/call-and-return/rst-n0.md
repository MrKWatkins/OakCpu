---
cpu: Z80
defined_by: Z80
form: RST N0
mnemonic: RST p
group: Call and Return
description: "Pushes the address of the next instruction onto the stack, then jumps to address {N0}."
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
  - encoding: C7
    mnemonic: RST 0x00
    operands: { N0: 0x00 }
    tstates: [11]
    undocumented: false
  - encoding: CF
    mnemonic: RST 0x08
    operands: { N0: 0x08 }
    tstates: [11]
    undocumented: false
  - encoding: D7
    mnemonic: RST 0x10
    operands: { N0: 0x10 }
    tstates: [11]
    undocumented: false
  - encoding: DF
    mnemonic: RST 0x18
    operands: { N0: 0x18 }
    tstates: [11]
    undocumented: false
  - encoding: E7
    mnemonic: RST 0x20
    operands: { N0: 0x20 }
    tstates: [11]
    undocumented: false
  - encoding: EF
    mnemonic: RST 0x28
    operands: { N0: 0x28 }
    tstates: [11]
    undocumented: false
  - encoding: F7
    mnemonic: RST 0x30
    operands: { N0: 0x30 }
    tstates: [11]
    undocumented: false
  - encoding: FF
    mnemonic: RST 0x38
    operands: { N0: 0x38 }
    tstates: [11]
    undocumented: false
  - encoding: DD C7
    mnemonic: RST 0x00
    operands: { N0: 0x00 }
    tstates: [15]
    undocumented: true
  - encoding: DD CF
    mnemonic: RST 0x08
    operands: { N0: 0x08 }
    tstates: [15]
    undocumented: true
  - encoding: DD D7
    mnemonic: RST 0x10
    operands: { N0: 0x10 }
    tstates: [15]
    undocumented: true
  - encoding: DD DF
    mnemonic: RST 0x18
    operands: { N0: 0x18 }
    tstates: [15]
    undocumented: true
  - encoding: DD E7
    mnemonic: RST 0x20
    operands: { N0: 0x20 }
    tstates: [15]
    undocumented: true
  - encoding: DD EF
    mnemonic: RST 0x28
    operands: { N0: 0x28 }
    tstates: [15]
    undocumented: true
  - encoding: DD F7
    mnemonic: RST 0x30
    operands: { N0: 0x30 }
    tstates: [15]
    undocumented: true
  - encoding: DD FF
    mnemonic: RST 0x38
    operands: { N0: 0x38 }
    tstates: [15]
    undocumented: true
  - encoding: FD C7
    mnemonic: RST 0x00
    operands: { N0: 0x00 }
    tstates: [15]
    undocumented: true
  - encoding: FD CF
    mnemonic: RST 0x08
    operands: { N0: 0x08 }
    tstates: [15]
    undocumented: true
  - encoding: FD D7
    mnemonic: RST 0x10
    operands: { N0: 0x10 }
    tstates: [15]
    undocumented: true
  - encoding: FD DF
    mnemonic: RST 0x18
    operands: { N0: 0x18 }
    tstates: [15]
    undocumented: true
  - encoding: FD E7
    mnemonic: RST 0x20
    operands: { N0: 0x20 }
    tstates: [15]
    undocumented: true
  - encoding: FD EF
    mnemonic: RST 0x28
    operands: { N0: 0x28 }
    tstates: [15]
    undocumented: true
  - encoding: FD F7
    mnemonic: RST 0x30
    operands: { N0: 0x30 }
    tstates: [15]
    undocumented: true
  - encoding: FD FF
    mnemonic: RST 0x38
    operands: { N0: 0x38 }
    tstates: [15]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# RST p

Pushes the address of the next instruction onto the stack, then jumps to address p.

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
| `RST 0x00` | `C7` | 11 |  |
| `RST 0x08` | `CF` | 11 |  |
| `RST 0x10` | `D7` | 11 |  |
| `RST 0x18` | `DF` | 11 |  |
| `RST 0x20` | `E7` | 11 |  |
| `RST 0x28` | `EF` | 11 |  |
| `RST 0x30` | `F7` | 11 |  |
| `RST 0x38` | `FF` | 11 |  |
| `RST 0x00` | `DD C7` | 15 | Undocumented. |
| `RST 0x08` | `DD CF` | 15 | Undocumented. |
| `RST 0x10` | `DD D7` | 15 | Undocumented. |
| `RST 0x18` | `DD DF` | 15 | Undocumented. |
| `RST 0x20` | `DD E7` | 15 | Undocumented. |
| `RST 0x28` | `DD EF` | 15 | Undocumented. |
| `RST 0x30` | `DD F7` | 15 | Undocumented. |
| `RST 0x38` | `DD FF` | 15 | Undocumented. |
| `RST 0x00` | `FD C7` | 15 | Undocumented. |
| `RST 0x08` | `FD CF` | 15 | Undocumented. |
| `RST 0x10` | `FD D7` | 15 | Undocumented. |
| `RST 0x18` | `FD DF` | 15 | Undocumented. |
| `RST 0x20` | `FD E7` | 15 | Undocumented. |
| `RST 0x28` | `FD EF` | 15 | Undocumented. |
| `RST 0x30` | `FD F7` | 15 | Undocumented. |
| `RST 0x38` | `FD FF` | 15 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
