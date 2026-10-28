---
cpu: Z80
defined_by: Z80
form: INC RP0
mnemonic: INC rr
group: 16-Bit Arithmetic
description: "Increments {RP0}."
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
  - encoding: 03
    mnemonic: INC BC
    operands: { RP0: BC }
    tstates: [6]
    undocumented: false
  - encoding: 13
    mnemonic: INC DE
    operands: { RP0: DE }
    tstates: [6]
    undocumented: false
  - encoding: 23
    mnemonic: INC HL
    operands: { RP0: HL }
    tstates: [6]
    undocumented: false
  - encoding: 33
    mnemonic: INC SP
    operands: { RP0: SP }
    tstates: [6]
    undocumented: false
  - encoding: DD 03
    mnemonic: INC BC
    operands: { RP0: BC }
    tstates: [10]
    undocumented: true
  - encoding: DD 13
    mnemonic: INC DE
    operands: { RP0: DE }
    tstates: [10]
    undocumented: true
  - encoding: DD 23
    mnemonic: INC IX
    operands: { RP0: IX }
    tstates: [10]
    undocumented: false
  - encoding: DD 33
    mnemonic: INC SP
    operands: { RP0: SP }
    tstates: [10]
    undocumented: true
  - encoding: FD 03
    mnemonic: INC BC
    operands: { RP0: BC }
    tstates: [10]
    undocumented: true
  - encoding: FD 13
    mnemonic: INC DE
    operands: { RP0: DE }
    tstates: [10]
    undocumented: true
  - encoding: FD 23
    mnemonic: INC IY
    operands: { RP0: IY }
    tstates: [10]
    undocumented: false
  - encoding: FD 33
    mnemonic: INC SP
    operands: { RP0: SP }
    tstates: [10]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# INC rr

Increments rr.

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
| `INC BC` | `03` | 6 |  |
| `INC DE` | `13` | 6 |  |
| `INC HL` | `23` | 6 |  |
| `INC SP` | `33` | 6 |  |
| `INC BC` | `DD 03` | 10 | Undocumented. |
| `INC DE` | `DD 13` | 10 | Undocumented. |
| `INC IX` | `DD 23` | 10 |  |
| `INC SP` | `DD 33` | 10 | Undocumented. |
| `INC BC` | `FD 03` | 10 | Undocumented. |
| `INC DE` | `FD 13` | 10 | Undocumented. |
| `INC IY` | `FD 23` | 10 |  |
| `INC SP` | `FD 33` | 10 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
