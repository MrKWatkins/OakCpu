---
cpu: Z80N
defined_by: Z80
form: LD R0, (RP0)
mnemonic: LD r, (rr)
group: 8-Bit Load
description: "Loads the byte at ({RP0}) into {R0}."
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
  - encoding: 46
    mnemonic: LD B, (HL)
    operands: { R0: B, RP0: HL }
    tstates: [7]
    undocumented: false
  - encoding: 4E
    mnemonic: LD C, (HL)
    operands: { R0: C, RP0: HL }
    tstates: [7]
    undocumented: false
  - encoding: 56
    mnemonic: LD D, (HL)
    operands: { R0: D, RP0: HL }
    tstates: [7]
    undocumented: false
  - encoding: 5E
    mnemonic: LD E, (HL)
    operands: { R0: E, RP0: HL }
    tstates: [7]
    undocumented: false
  - encoding: 66
    mnemonic: LD H, (HL)
    operands: { R0: H, RP0: HL }
    tstates: [7]
    undocumented: false
  - encoding: 6E
    mnemonic: LD L, (HL)
    operands: { R0: L, RP0: HL }
    tstates: [7]
    undocumented: false
  - encoding: 7E
    mnemonic: LD A, (HL)
    operands: { R0: A, RP0: HL }
    tstates: [7]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD r, (rr)

Loads the byte at (rr) into r.

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
| `LD B, (HL)` | `46` | 7 |  |
| `LD C, (HL)` | `4E` | 7 |  |
| `LD D, (HL)` | `56` | 7 |  |
| `LD E, (HL)` | `5E` | 7 |  |
| `LD H, (HL)` | `66` | 7 |  |
| `LD L, (HL)` | `6E` | 7 |  |
| `LD A, (HL)` | `7E` | 7 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
