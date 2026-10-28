---
cpu: Z80
defined_by: Z80
form: CALL nn
mnemonic: CALL nn
group: Call and Return
description: Pushes the address of the next instruction onto the stack, then jumps to nn.
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
  - encoding: CD nn
    mnemonic: CALL nn
    tstates: [17]
    undocumented: false
  - encoding: DD CD nn
    mnemonic: CALL nn
    tstates: [21]
    undocumented: true
  - encoding: FD CD nn
    mnemonic: CALL nn
    tstates: [21]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# CALL nn

Pushes the address of the next instruction onto the stack, then jumps to nn.

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
| `CALL nn` | `CD nn` | 17 |  |
| `CALL nn` | `DD CD nn` | 21 | Undocumented. |
| `CALL nn` | `FD CD nn` | 21 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
