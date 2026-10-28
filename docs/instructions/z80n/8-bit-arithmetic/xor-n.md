---
cpu: Z80N
defined_by: Z80
form: XOR n
mnemonic: XOR n
group: 8-Bit Arithmetic
description: Sets A to the bitwise exclusive OR of A and n.
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of A.
  - flag: Z
    effect: changed
    description: Set if A is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of A.
  - flag: H
    effect: reset
    description: Reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of A.
  - flag: PV
    effect: changed
    description: Set if A has even parity, otherwise reset.
  - flag: N
    effect: reset
    description: Reset.
  - flag: C
    effect: reset
    description: Reset.
opcodes:
  - encoding: EE n
    mnemonic: XOR n
    tstates: [7]
    undocumented: false
  - encoding: DD EE n
    mnemonic: XOR n
    tstates: [11]
    undocumented: true
  - encoding: FD EE n
    mnemonic: XOR n
    tstates: [11]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# XOR n

Sets A to the bitwise exclusive OR of A and n.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of A. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of A. |
| H | Reset. |
| X | Copied from bit 3 of A. |
| PV | Set if A has even parity, otherwise reset. |
| N | Reset. |
| C | Reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `XOR n` | `EE n` | 7 |  |
| `XOR n` | `DD EE n` | 11 | Undocumented. |
| `XOR n` | `FD EE n` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
