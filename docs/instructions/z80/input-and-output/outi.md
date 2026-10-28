---
cpu: Z80
defined_by: Z80
form: OUTI
mnemonic: OUTI
group: Input and Output
description: Decrements B, then writes the byte at (HL) to the port addressed by BC and increments HL.
undocumented: false
flags:
  - flag: S
    effect: changed
    description: Set if bit 7 of B is set, otherwise reset.
  - flag: Z
    effect: changed
    description: Set if B is zero, otherwise reset.
  - flag: Y
    effect: changed
    description: Copied from bit 5 of B.
  - flag: H
    effect: changed
    description: "Set if (the value read + (L & 0xFF)) is greater than 0xFF, otherwise reset."
  - flag: X
    effect: changed
    description: Copied from bit 3 of B.
  - flag: PV
    effect: changed
    description: "Set if (((the value read + (L & 0xFF)) & 0x07) ^ B) has even parity, otherwise reset."
  - flag: N
    effect: changed
    description: Set if bit 7 of the value read is set, otherwise reset.
  - flag: C
    effect: changed
    description: "Set if (the value read + (L & 0xFF)) is greater than 0xFF, otherwise reset."
opcodes:
  - encoding: ED A3
    mnemonic: OUTI
    tstates: [16]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# OUTI

Decrements B, then writes the byte at (HL) to the port addressed by BC and increments HL.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Set if bit 7 of B is set, otherwise reset. |
| Z | Set if B is zero, otherwise reset. |
| Y | Copied from bit 5 of B. |
| H | Set if (the value read + (L & 0xFF)) is greater than 0xFF, otherwise reset. |
| X | Copied from bit 3 of B. |
| PV | Set if (((the value read + (L & 0xFF)) & 0x07) ^ B) has even parity, otherwise reset. |
| N | Set if bit 7 of the value read is set, otherwise reset. |
| C | Set if (the value read + (L & 0xFF)) is greater than 0xFF, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `OUTI` | `ED A3` | 16 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
