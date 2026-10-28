---
cpu: Z80
defined_by: Z80
form: XOR (HL)
mnemonic: XOR (HL)
group: 8-Bit Arithmetic
description: Sets A to the bitwise exclusive OR of A and the byte at (HL).
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
  - encoding: AE
    mnemonic: XOR (HL)
    tstates: [7]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# XOR (HL)

Sets A to the bitwise exclusive OR of A and the byte at (HL).

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
| `XOR (HL)` | `AE` | 7 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
