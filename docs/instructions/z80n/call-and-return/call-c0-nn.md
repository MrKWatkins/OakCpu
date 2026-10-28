---
cpu: Z80N
defined_by: Z80
form: CALL C0, nn
mnemonic: CALL cc, nn
group: Call and Return
description: "If condition {C0} is met, pushes the address of the next instruction onto the stack and jumps to nn; otherwise continues with the next instruction."
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
  - encoding: C4 nn
    mnemonic: CALL NZ, nn
    operands: { C0: NZ }
    tstates: [10, 17]
    undocumented: false
  - encoding: CC nn
    mnemonic: CALL Z, nn
    operands: { C0: Z }
    tstates: [10, 17]
    undocumented: false
  - encoding: D4 nn
    mnemonic: CALL NC, nn
    operands: { C0: NC }
    tstates: [10, 17]
    undocumented: false
  - encoding: DC nn
    mnemonic: CALL C, nn
    operands: { C0: C }
    tstates: [10, 17]
    undocumented: false
  - encoding: E4 nn
    mnemonic: CALL PO, nn
    operands: { C0: PO }
    tstates: [10, 17]
    undocumented: false
  - encoding: EC nn
    mnemonic: CALL PE, nn
    operands: { C0: PE }
    tstates: [10, 17]
    undocumented: false
  - encoding: F4 nn
    mnemonic: CALL P, nn
    operands: { C0: P }
    tstates: [10, 17]
    undocumented: false
  - encoding: FC nn
    mnemonic: CALL M, nn
    operands: { C0: M }
    tstates: [10, 17]
    undocumented: false
  - encoding: DD C4 nn
    mnemonic: CALL NZ, nn
    operands: { C0: NZ }
    tstates: [14, 21]
    undocumented: true
  - encoding: DD CC nn
    mnemonic: CALL Z, nn
    operands: { C0: Z }
    tstates: [14, 21]
    undocumented: true
  - encoding: DD D4 nn
    mnemonic: CALL NC, nn
    operands: { C0: NC }
    tstates: [14, 21]
    undocumented: true
  - encoding: DD DC nn
    mnemonic: CALL C, nn
    operands: { C0: C }
    tstates: [14, 21]
    undocumented: true
  - encoding: DD E4 nn
    mnemonic: CALL PO, nn
    operands: { C0: PO }
    tstates: [14, 21]
    undocumented: true
  - encoding: DD EC nn
    mnemonic: CALL PE, nn
    operands: { C0: PE }
    tstates: [14, 21]
    undocumented: true
  - encoding: DD F4 nn
    mnemonic: CALL P, nn
    operands: { C0: P }
    tstates: [14, 21]
    undocumented: true
  - encoding: DD FC nn
    mnemonic: CALL M, nn
    operands: { C0: M }
    tstates: [14, 21]
    undocumented: true
  - encoding: FD C4 nn
    mnemonic: CALL NZ, nn
    operands: { C0: NZ }
    tstates: [14, 21]
    undocumented: true
  - encoding: FD CC nn
    mnemonic: CALL Z, nn
    operands: { C0: Z }
    tstates: [14, 21]
    undocumented: true
  - encoding: FD D4 nn
    mnemonic: CALL NC, nn
    operands: { C0: NC }
    tstates: [14, 21]
    undocumented: true
  - encoding: FD DC nn
    mnemonic: CALL C, nn
    operands: { C0: C }
    tstates: [14, 21]
    undocumented: true
  - encoding: FD E4 nn
    mnemonic: CALL PO, nn
    operands: { C0: PO }
    tstates: [14, 21]
    undocumented: true
  - encoding: FD EC nn
    mnemonic: CALL PE, nn
    operands: { C0: PE }
    tstates: [14, 21]
    undocumented: true
  - encoding: FD F4 nn
    mnemonic: CALL P, nn
    operands: { C0: P }
    tstates: [14, 21]
    undocumented: true
  - encoding: FD FC nn
    mnemonic: CALL M, nn
    operands: { C0: M }
    tstates: [14, 21]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# CALL cc, nn

If condition cc is met, pushes the address of the next instruction onto the stack and jumps to nn; otherwise continues with the next instruction.

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
| `CALL NZ, nn` | `C4 nn` | 10 or 17 |  |
| `CALL Z, nn` | `CC nn` | 10 or 17 |  |
| `CALL NC, nn` | `D4 nn` | 10 or 17 |  |
| `CALL C, nn` | `DC nn` | 10 or 17 |  |
| `CALL PO, nn` | `E4 nn` | 10 or 17 |  |
| `CALL PE, nn` | `EC nn` | 10 or 17 |  |
| `CALL P, nn` | `F4 nn` | 10 or 17 |  |
| `CALL M, nn` | `FC nn` | 10 or 17 |  |
| `CALL NZ, nn` | `DD C4 nn` | 14 or 21 | Undocumented. |
| `CALL Z, nn` | `DD CC nn` | 14 or 21 | Undocumented. |
| `CALL NC, nn` | `DD D4 nn` | 14 or 21 | Undocumented. |
| `CALL C, nn` | `DD DC nn` | 14 or 21 | Undocumented. |
| `CALL PO, nn` | `DD E4 nn` | 14 or 21 | Undocumented. |
| `CALL PE, nn` | `DD EC nn` | 14 or 21 | Undocumented. |
| `CALL P, nn` | `DD F4 nn` | 14 or 21 | Undocumented. |
| `CALL M, nn` | `DD FC nn` | 14 or 21 | Undocumented. |
| `CALL NZ, nn` | `FD C4 nn` | 14 or 21 | Undocumented. |
| `CALL Z, nn` | `FD CC nn` | 14 or 21 | Undocumented. |
| `CALL NC, nn` | `FD D4 nn` | 14 or 21 | Undocumented. |
| `CALL C, nn` | `FD DC nn` | 14 or 21 | Undocumented. |
| `CALL PO, nn` | `FD E4 nn` | 14 or 21 | Undocumented. |
| `CALL PE, nn` | `FD EC nn` | 14 or 21 | Undocumented. |
| `CALL P, nn` | `FD F4 nn` | 14 or 21 | Undocumented. |
| `CALL M, nn` | `FD FC nn` | 14 or 21 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
