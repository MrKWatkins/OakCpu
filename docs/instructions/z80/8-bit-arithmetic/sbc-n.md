---
cpu: Z80
defined_by: Z80
form: SBC n
mnemonic: SBC n
group: 8-Bit Arithmetic
description: Subtracts n and the carry flag from A.
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the result.
  - flag: Z
    effect: changed
    description: Set if A is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of the result.
  - flag: H
    effect: changed
    description: Set if there was a borrow from bit 4, otherwise reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the result.
  - flag: PV
    effect: changed
    description: Set if the subtraction overflowed, otherwise reset.
  - flag: N
    effect: set
    description: Set.
  - flag: C
    effect: changed
    description: Set if bit 8 of the result is set, otherwise reset.
opcodes:
  - encoding: DE n
    mnemonic: SBC n
    tstates: [7]
    undocumented: false
  - encoding: DD DE n
    mnemonic: SBC n
    tstates: [11]
    undocumented: true
  - encoding: FD DE n
    mnemonic: SBC n
    tstates: [11]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# SBC n

Subtracts n and the carry flag from A.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of the result. |
| H | Set if there was a borrow from bit 4, otherwise reset. |
| X | Copied from bit 3 of the result. |
| PV | Set if the subtraction overflowed, otherwise reset. |
| N | Set. |
| C | Set if bit 8 of the result is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `SBC n` | `DE n` | 7 |  |
| `SBC n` | `DD DE n` | 11 | Undocumented. |
| `SBC n` | `FD DE n` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
