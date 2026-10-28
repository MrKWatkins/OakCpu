---
cpu: Z80
defined_by: Z80
form: CP n
mnemonic: CP n
group: 8-Bit Arithmetic
description: Compares A with n by subtracting n from A, setting the flags from the result and leaving A unchanged.
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Copied from bit 7 of the result.
  - flag: Z
    effect: changed
    description: Set if the result is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of the value read.
  - flag: H
    effect: changed
    description: Set if there was a borrow from bit 4, otherwise reset.
  - flag: X
    effect: changed
    description: Copied from bit 3 of the value read.
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
  - encoding: FE n
    mnemonic: CP n
    tstates: [7]
    undocumented: false
  - encoding: DD FE n
    mnemonic: CP n
    tstates: [11]
    undocumented: true
  - encoding: FD FE n
    mnemonic: CP n
    tstates: [11]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# CP n

Compares A with n by subtracting n from A, setting the flags from the result and leaving A unchanged.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of the value read. |
| H | Set if there was a borrow from bit 4, otherwise reset. |
| X | Copied from bit 3 of the value read. |
| PV | Set if the subtraction overflowed, otherwise reset. |
| N | Set. |
| C | Set if bit 8 of the result is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `CP n` | `FE n` | 7 |  |
| `CP n` | `DD FE n` | 11 | Undocumented. |
| `CP n` | `FD FE n` | 11 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
