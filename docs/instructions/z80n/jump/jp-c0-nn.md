---
cpu: Z80N
defined_by: Z80
form: JP C0, nn
mnemonic: JP cc, nn
group: Jump
description: "Jumps to nn if condition {C0} is met; otherwise continues with the next instruction."
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
  - encoding: C2 nn
    mnemonic: JP NZ, nn
    operands: { C0: NZ }
    tstates: [10]
    undocumented: false
  - encoding: CA nn
    mnemonic: JP Z, nn
    operands: { C0: Z }
    tstates: [10]
    undocumented: false
  - encoding: D2 nn
    mnemonic: JP NC, nn
    operands: { C0: NC }
    tstates: [10]
    undocumented: false
  - encoding: DA nn
    mnemonic: JP C, nn
    operands: { C0: C }
    tstates: [10]
    undocumented: false
  - encoding: E2 nn
    mnemonic: JP PO, nn
    operands: { C0: PO }
    tstates: [10]
    undocumented: false
  - encoding: EA nn
    mnemonic: JP PE, nn
    operands: { C0: PE }
    tstates: [10]
    undocumented: false
  - encoding: F2 nn
    mnemonic: JP P, nn
    operands: { C0: P }
    tstates: [10]
    undocumented: false
  - encoding: FA nn
    mnemonic: JP M, nn
    operands: { C0: M }
    tstates: [10]
    undocumented: false
  - encoding: DD C2 nn
    mnemonic: JP NZ, nn
    operands: { C0: NZ }
    tstates: [14]
    undocumented: true
  - encoding: DD CA nn
    mnemonic: JP Z, nn
    operands: { C0: Z }
    tstates: [14]
    undocumented: true
  - encoding: DD D2 nn
    mnemonic: JP NC, nn
    operands: { C0: NC }
    tstates: [14]
    undocumented: true
  - encoding: DD DA nn
    mnemonic: JP C, nn
    operands: { C0: C }
    tstates: [14]
    undocumented: true
  - encoding: DD E2 nn
    mnemonic: JP PO, nn
    operands: { C0: PO }
    tstates: [14]
    undocumented: true
  - encoding: DD EA nn
    mnemonic: JP PE, nn
    operands: { C0: PE }
    tstates: [14]
    undocumented: true
  - encoding: DD F2 nn
    mnemonic: JP P, nn
    operands: { C0: P }
    tstates: [14]
    undocumented: true
  - encoding: DD FA nn
    mnemonic: JP M, nn
    operands: { C0: M }
    tstates: [14]
    undocumented: true
  - encoding: FD C2 nn
    mnemonic: JP NZ, nn
    operands: { C0: NZ }
    tstates: [14]
    undocumented: true
  - encoding: FD CA nn
    mnemonic: JP Z, nn
    operands: { C0: Z }
    tstates: [14]
    undocumented: true
  - encoding: FD D2 nn
    mnemonic: JP NC, nn
    operands: { C0: NC }
    tstates: [14]
    undocumented: true
  - encoding: FD DA nn
    mnemonic: JP C, nn
    operands: { C0: C }
    tstates: [14]
    undocumented: true
  - encoding: FD E2 nn
    mnemonic: JP PO, nn
    operands: { C0: PO }
    tstates: [14]
    undocumented: true
  - encoding: FD EA nn
    mnemonic: JP PE, nn
    operands: { C0: PE }
    tstates: [14]
    undocumented: true
  - encoding: FD F2 nn
    mnemonic: JP P, nn
    operands: { C0: P }
    tstates: [14]
    undocumented: true
  - encoding: FD FA nn
    mnemonic: JP M, nn
    operands: { C0: M }
    tstates: [14]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# JP cc, nn

Jumps to nn if condition cc is met; otherwise continues with the next instruction.

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
| `JP NZ, nn` | `C2 nn` | 10 |  |
| `JP Z, nn` | `CA nn` | 10 |  |
| `JP NC, nn` | `D2 nn` | 10 |  |
| `JP C, nn` | `DA nn` | 10 |  |
| `JP PO, nn` | `E2 nn` | 10 |  |
| `JP PE, nn` | `EA nn` | 10 |  |
| `JP P, nn` | `F2 nn` | 10 |  |
| `JP M, nn` | `FA nn` | 10 |  |
| `JP NZ, nn` | `DD C2 nn` | 14 | Undocumented. |
| `JP Z, nn` | `DD CA nn` | 14 | Undocumented. |
| `JP NC, nn` | `DD D2 nn` | 14 | Undocumented. |
| `JP C, nn` | `DD DA nn` | 14 | Undocumented. |
| `JP PO, nn` | `DD E2 nn` | 14 | Undocumented. |
| `JP PE, nn` | `DD EA nn` | 14 | Undocumented. |
| `JP P, nn` | `DD F2 nn` | 14 | Undocumented. |
| `JP M, nn` | `DD FA nn` | 14 | Undocumented. |
| `JP NZ, nn` | `FD C2 nn` | 14 | Undocumented. |
| `JP Z, nn` | `FD CA nn` | 14 | Undocumented. |
| `JP NC, nn` | `FD D2 nn` | 14 | Undocumented. |
| `JP C, nn` | `FD DA nn` | 14 | Undocumented. |
| `JP PO, nn` | `FD E2 nn` | 14 | Undocumented. |
| `JP PE, nn` | `FD EA nn` | 14 | Undocumented. |
| `JP P, nn` | `FD F2 nn` | 14 | Undocumented. |
| `JP M, nn` | `FD FA nn` | 14 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
