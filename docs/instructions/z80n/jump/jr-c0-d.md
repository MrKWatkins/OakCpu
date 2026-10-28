---
cpu: Z80N
defined_by: Z80
form: JR C0, d
mnemonic: JR cc, d
group: Jump
description: "Jumps to the address d bytes from the next instruction if condition {C0} is met; otherwise continues with the next instruction."
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
  - encoding: 20 d
    mnemonic: JR NZ, d
    operands: { C0: NZ }
    tstates: [7, 12]
    undocumented: false
  - encoding: 28 d
    mnemonic: JR Z, d
    operands: { C0: Z }
    tstates: [7, 12]
    undocumented: false
  - encoding: 30 d
    mnemonic: JR NC, d
    operands: { C0: NC }
    tstates: [7, 12]
    undocumented: false
  - encoding: 38 d
    mnemonic: JR C, d
    operands: { C0: C }
    tstates: [7, 12]
    undocumented: false
  - encoding: DD 20 d
    mnemonic: JR NZ, d
    operands: { C0: NZ }
    tstates: [11, 16]
    undocumented: true
  - encoding: DD 28 d
    mnemonic: JR Z, d
    operands: { C0: Z }
    tstates: [11, 16]
    undocumented: true
  - encoding: DD 30 d
    mnemonic: JR NC, d
    operands: { C0: NC }
    tstates: [11, 16]
    undocumented: true
  - encoding: DD 38 d
    mnemonic: JR C, d
    operands: { C0: C }
    tstates: [11, 16]
    undocumented: true
  - encoding: FD 20 d
    mnemonic: JR NZ, d
    operands: { C0: NZ }
    tstates: [11, 16]
    undocumented: true
  - encoding: FD 28 d
    mnemonic: JR Z, d
    operands: { C0: Z }
    tstates: [11, 16]
    undocumented: true
  - encoding: FD 30 d
    mnemonic: JR NC, d
    operands: { C0: NC }
    tstates: [11, 16]
    undocumented: true
  - encoding: FD 38 d
    mnemonic: JR C, d
    operands: { C0: C }
    tstates: [11, 16]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# JR cc, d

Jumps to the address d bytes from the next instruction if condition cc is met; otherwise continues with the next instruction.

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
| `JR NZ, d` | `20 d` | 7 or 12 |  |
| `JR Z, d` | `28 d` | 7 or 12 |  |
| `JR NC, d` | `30 d` | 7 or 12 |  |
| `JR C, d` | `38 d` | 7 or 12 |  |
| `JR NZ, d` | `DD 20 d` | 11 or 16 | Undocumented. |
| `JR Z, d` | `DD 28 d` | 11 or 16 | Undocumented. |
| `JR NC, d` | `DD 30 d` | 11 or 16 | Undocumented. |
| `JR C, d` | `DD 38 d` | 11 or 16 | Undocumented. |
| `JR NZ, d` | `FD 20 d` | 11 or 16 | Undocumented. |
| `JR Z, d` | `FD 28 d` | 11 or 16 | Undocumented. |
| `JR NC, d` | `FD 30 d` | 11 or 16 | Undocumented. |
| `JR C, d` | `FD 38 d` | 11 or 16 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
