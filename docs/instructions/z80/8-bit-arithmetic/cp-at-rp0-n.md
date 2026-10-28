---
cpu: Z80
defined_by: Z80
form: CP (RP0 + n)
mnemonic: CP (rr + n)
group: 8-Bit Arithmetic
description: "Compares A with the byte at ({RP0} + n) by subtracting it from A, setting the flags from the result and leaving A unchanged."
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
  - encoding: DD BE n
    mnemonic: CP (IX + n)
    operands: { RP0: IX }
    tstates: [19]
    undocumented: false
  - encoding: FD BE n
    mnemonic: CP (IY + n)
    operands: { RP0: IY }
    tstates: [19]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# CP (rr + n)

Compares A with the byte at (rr + n) by subtracting it from A, setting the flags from the result and leaving A unchanged.

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
| `CP (IX + n)` | `DD BE n` | 19 |  |
| `CP (IY + n)` | `FD BE n` | 19 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
