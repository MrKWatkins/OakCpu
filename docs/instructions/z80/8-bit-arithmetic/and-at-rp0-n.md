---
cpu: Z80
defined_by: Z80
form: AND (RP0 + n)
mnemonic: AND (rr + n)
group: 8-Bit Arithmetic
description: "Sets A to the bitwise AND of A and the byte at ({RP0} + n)."
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
    effect: set
    description: Set.
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
  - encoding: DD A6 n
    mnemonic: AND (IX + n)
    operands: { RP0: IX }
    tstates: [19]
    undocumented: false
  - encoding: FD A6 n
    mnemonic: AND (IY + n)
    operands: { RP0: IY }
    tstates: [19]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# AND (rr + n)

Sets A to the bitwise AND of A and the byte at (rr + n).

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of A. |
| Z | Set if A is zero, otherwise reset. |
| Y | Copied from bit 5 of A. |
| H | Set. |
| X | Copied from bit 3 of A. |
| PV | Set if A has even parity, otherwise reset. |
| N | Reset. |
| C | Reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `AND (IX + n)` | `DD A6 n` | 19 |  |
| `AND (IY + n)` | `FD A6 n` | 19 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
