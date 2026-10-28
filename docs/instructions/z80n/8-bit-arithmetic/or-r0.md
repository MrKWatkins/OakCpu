---
cpu: Z80N
defined_by: Z80
form: OR R0
mnemonic: OR r
group: 8-Bit Arithmetic
description: "Sets A to the bitwise OR of A and {R0}."
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
  - encoding: B0
    mnemonic: OR B
    operands: { R0: B }
    tstates: [4]
    undocumented: false
  - encoding: B1
    mnemonic: OR C
    operands: { R0: C }
    tstates: [4]
    undocumented: false
  - encoding: B2
    mnemonic: OR D
    operands: { R0: D }
    tstates: [4]
    undocumented: false
  - encoding: B3
    mnemonic: OR E
    operands: { R0: E }
    tstates: [4]
    undocumented: false
  - encoding: B4
    mnemonic: OR H
    operands: { R0: H }
    tstates: [4]
    undocumented: false
  - encoding: B5
    mnemonic: OR L
    operands: { R0: L }
    tstates: [4]
    undocumented: false
  - encoding: B7
    mnemonic: OR A
    operands: { R0: A }
    tstates: [4]
    undocumented: false
  - encoding: DD B0
    mnemonic: OR B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: DD B1
    mnemonic: OR C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: DD B2
    mnemonic: OR D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: DD B3
    mnemonic: OR E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: DD B4
    mnemonic: OR IXH
    operands: { R0: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD B5
    mnemonic: OR IXL
    operands: { R0: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD B7
    mnemonic: OR A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
  - encoding: FD B0
    mnemonic: OR B
    operands: { R0: B }
    tstates: [8]
    undocumented: true
  - encoding: FD B1
    mnemonic: OR C
    operands: { R0: C }
    tstates: [8]
    undocumented: true
  - encoding: FD B2
    mnemonic: OR D
    operands: { R0: D }
    tstates: [8]
    undocumented: true
  - encoding: FD B3
    mnemonic: OR E
    operands: { R0: E }
    tstates: [8]
    undocumented: true
  - encoding: FD B4
    mnemonic: OR IYH
    operands: { R0: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD B5
    mnemonic: OR IYL
    operands: { R0: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD B7
    mnemonic: OR A
    operands: { R0: A }
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# OR r

Sets A to the bitwise OR of A and r.

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
| `OR B` | `B0` | 4 |  |
| `OR C` | `B1` | 4 |  |
| `OR D` | `B2` | 4 |  |
| `OR E` | `B3` | 4 |  |
| `OR H` | `B4` | 4 |  |
| `OR L` | `B5` | 4 |  |
| `OR A` | `B7` | 4 |  |
| `OR B` | `DD B0` | 8 | Undocumented. |
| `OR C` | `DD B1` | 8 | Undocumented. |
| `OR D` | `DD B2` | 8 | Undocumented. |
| `OR E` | `DD B3` | 8 | Undocumented. |
| `OR IXH` | `DD B4` | 8 | Undocumented. |
| `OR IXL` | `DD B5` | 8 | Undocumented. |
| `OR A` | `DD B7` | 8 | Undocumented. |
| `OR B` | `FD B0` | 8 | Undocumented. |
| `OR C` | `FD B1` | 8 | Undocumented. |
| `OR D` | `FD B2` | 8 | Undocumented. |
| `OR E` | `FD B3` | 8 | Undocumented. |
| `OR IYH` | `FD B4` | 8 | Undocumented. |
| `OR IYL` | `FD B5` | 8 | Undocumented. |
| `OR A` | `FD B7` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
