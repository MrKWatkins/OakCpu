---
cpu: Z80
defined_by: Z80
form: CP R0
mnemonic: CP r
group: 8-Bit Arithmetic
description: "Compares A with {R0} by subtracting {R0} from A, setting the flags from the result and leaving A unchanged."
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
    description: "Copied from bit 5 of {R0}."
  - flag: H
    effect: changed
    description: Set if there was a borrow from bit 4, otherwise reset.
  - flag: X
    effect: changed
    description: "Copied from bit 3 of {R0}."
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
  - encoding: B8
    mnemonic: CP B
    operands: { R0: B }
    tstates: [4]
    undocumented: false
  - encoding: B9
    mnemonic: CP C
    operands: { R0: C }
    tstates: [4]
    undocumented: false
  - encoding: BA
    mnemonic: CP D
    operands: { R0: D }
    tstates: [4]
    undocumented: false
  - encoding: BB
    mnemonic: CP E
    operands: { R0: E }
    tstates: [4]
    undocumented: false
  - encoding: BC
    mnemonic: CP H
    operands: { R0: H }
    tstates: [4]
    undocumented: false
  - encoding: BD
    mnemonic: CP L
    operands: { R0: L }
    tstates: [4]
    undocumented: false
  - encoding: BF
    mnemonic: CP A
    operands: { R0: A }
    tstates: [4]
    undocumented: false
  - encoding: DD B8
    mnemonic: CP B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: DD B9
    mnemonic: CP C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: DD BA
    mnemonic: CP D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: DD BB
    mnemonic: CP E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: DD BC
    mnemonic: CP IXH
    operands: { R0: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD BD
    mnemonic: CP IXL
    operands: { R0: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD BF
    mnemonic: CP A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
  - encoding: FD B8
    mnemonic: CP B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: FD B9
    mnemonic: CP C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: FD BA
    mnemonic: CP D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: FD BB
    mnemonic: CP E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: FD BC
    mnemonic: CP IYH
    operands: { R0: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD BD
    mnemonic: CP IYL
    operands: { R0: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD BF
    mnemonic: CP A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# CP r

Compares A with r by subtracting r from A, setting the flags from the result and leaving A unchanged.

## Flags

| Flag | Effect |
| ---- | ------ |
| S | Copied from bit 7 of the result. |
| Z | Set if the result is zero, otherwise reset. |
| Y | Copied from bit 5 of r. |
| H | Set if there was a borrow from bit 4, otherwise reset. |
| X | Copied from bit 3 of r. |
| PV | Set if the subtraction overflowed, otherwise reset. |
| N | Set. |
| C | Set if bit 8 of the result is set, otherwise reset. |

## Opcodes

| Opcode | Encoding | T-states | Notes |
| ------ | -------- | -------- | ----- |
| `CP B` | `B8` | 4 |  |
| `CP C` | `B9` | 4 |  |
| `CP D` | `BA` | 4 |  |
| `CP E` | `BB` | 4 |  |
| `CP H` | `BC` | 4 |  |
| `CP L` | `BD` | 4 |  |
| `CP A` | `BF` | 4 |  |
| `CP B` | `DD B8` | 8 | Undocumented. |
| `CP C` | `DD B9` | 8 | Undocumented. |
| `CP D` | `DD BA` | 8 | Undocumented. |
| `CP E` | `DD BB` | 8 | Undocumented. |
| `CP IXH` | `DD BC` | 8 | Undocumented. |
| `CP IXL` | `DD BD` | 8 | Undocumented. |
| `CP A` | `DD BF` | 8 | Undocumented. |
| `CP B` | `FD B8` | 8 | Undocumented. |
| `CP C` | `FD B9` | 8 | Undocumented. |
| `CP D` | `FD BA` | 8 | Undocumented. |
| `CP E` | `FD BB` | 8 | Undocumented. |
| `CP IYH` | `FD BC` | 8 | Undocumented. |
| `CP IYL` | `FD BD` | 8 | Undocumented. |
| `CP A` | `FD BF` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
