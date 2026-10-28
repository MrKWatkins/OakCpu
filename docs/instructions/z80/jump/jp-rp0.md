---
cpu: Z80
defined_by: Z80
form: JP RP0
mnemonic: JP rr
group: Jump
description: "Jumps to the address in {RP0}."
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
  - encoding: E9
    mnemonic: JP HL
    operands: { RP0: HL }
    tstates: [4]
    undocumented: false
  - encoding: DD E9
    mnemonic: JP IX
    operands: { RP0: IX }
    tstates: [8]
    undocumented: false
  - encoding: FD E9
    mnemonic: JP IY
    operands: { RP0: IY }
    tstates: [8]
    undocumented: false
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# JP rr

Jumps to the address in rr.

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
| `JP HL` | `E9` | 4 |  |
| `JP IX` | `DD E9` | 8 |  |
| `JP IY` | `FD E9` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
