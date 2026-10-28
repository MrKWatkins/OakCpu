---
cpu: Z80N
defined_by: Z80
form: IN R0, (C)
mnemonic: IN r, (C)
group: Input and Output
description: "Reads a byte from the port addressed by BC into {R0}."
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the value read.
  - flag: Z
    effect: changed
    description: Set if the value read is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of the value read.
  - flag: H
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the value read.
  - flag: PV
    effect: changed
    description: Set if the value read has even parity, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: unaffected
    description: Unaffected.
opcodes:
  - encoding: ED 40
    mnemonic: IN B, (C)
    operands: { R0: B }
    tstates: [12]
    undocumented: false
  - encoding: ED 48
    mnemonic: IN C, (C)
    operands: { R0: C }
    tstates: [12]
    undocumented: false
  - encoding: ED 50
    mnemonic: IN D, (C)
    operands: { R0: D }
    tstates: [12]
    undocumented: false
  - encoding: ED 58
    mnemonic: IN E, (C)
    operands: { R0: E }
    tstates: [12]
    undocumented: false
  - encoding: ED 60
    mnemonic: IN H, (C)
    operands: { R0: H }
    tstates: [12]
    undocumented: false
  - encoding: ED 68
    mnemonic: IN L, (C)
    operands: { R0: L }
    tstates: [12]
    undocumented: false
  - encoding: ED 78
    mnemonic: IN A, (C)
    operands: { R0: A }
    tstates: [12]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# IN r, (C)

Reads a byte from the port addressed by BC into r.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the value read. |
| Z | Set if the value read is zero, otherwise reset. |
| Y | Copied from bit 5 of the value read. |
| H | Reset. |
| X | Copied from bit 3 of the value read. |
| PV | Set if the value read has even parity, otherwise reset. |
| N | Reset. |
| C | Unaffected. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `IN B, (C)` | `ED 40` | 12 |  |
| `IN C, (C)` | `ED 48` | 12 |  |
| `IN D, (C)` | `ED 50` | 12 |  |
| `IN E, (C)` | `ED 58` | 12 |  |
| `IN H, (C)` | `ED 60` | 12 |  |
| `IN L, (C)` | `ED 68` | 12 |  |
| `IN A, (C)` | `ED 78` | 12 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
