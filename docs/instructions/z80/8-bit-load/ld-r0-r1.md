---
cpu: Z80
defined_by: Z80
form: LD R0, R1
mnemonic: LD r, r'
group: 8-Bit Load
description: "Copies {R1} into {R0}."
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
  - encoding: 40
    mnemonic: LD B, B
    operands: { R0: B, R1: B }
    tstates: [4]
    undocumented: false
  - encoding: 41
    mnemonic: LD B, C
    operands: { R0: B, R1: C }
    tstates: [4]
    undocumented: false
  - encoding: 42
    mnemonic: LD B, D
    operands: { R0: B, R1: D }
    tstates: [4]
    undocumented: false
  - encoding: 43
    mnemonic: LD B, E
    operands: { R0: B, R1: E }
    tstates: [4]
    undocumented: false
  - encoding: 44
    mnemonic: LD B, H
    operands: { R0: B, R1: H }
    tstates: [4]
    undocumented: false
  - encoding: 45
    mnemonic: LD B, L
    operands: { R0: B, R1: L }
    tstates: [4]
    undocumented: false
  - encoding: 47
    mnemonic: LD B, A
    operands: { R0: B, R1: A }
    tstates: [4]
    undocumented: false
  - encoding: 48
    mnemonic: LD C, B
    operands: { R0: C, R1: B }
    tstates: [4]
    undocumented: false
  - encoding: 49
    mnemonic: LD C, C
    operands: { R0: C, R1: C }
    tstates: [4]
    undocumented: false
  - encoding: 4A
    mnemonic: LD C, D
    operands: { R0: C, R1: D }
    tstates: [4]
    undocumented: false
  - encoding: 4B
    mnemonic: LD C, E
    operands: { R0: C, R1: E }
    tstates: [4]
    undocumented: false
  - encoding: 4C
    mnemonic: LD C, H
    operands: { R0: C, R1: H }
    tstates: [4]
    undocumented: false
  - encoding: 4D
    mnemonic: LD C, L
    operands: { R0: C, R1: L }
    tstates: [4]
    undocumented: false
  - encoding: 4F
    mnemonic: LD C, A
    operands: { R0: C, R1: A }
    tstates: [4]
    undocumented: false
  - encoding: 50
    mnemonic: LD D, B
    operands: { R0: D, R1: B }
    tstates: [4]
    undocumented: false
  - encoding: 51
    mnemonic: LD D, C
    operands: { R0: D, R1: C }
    tstates: [4]
    undocumented: false
  - encoding: 52
    mnemonic: LD D, D
    operands: { R0: D, R1: D }
    tstates: [4]
    undocumented: false
  - encoding: 53
    mnemonic: LD D, E
    operands: { R0: D, R1: E }
    tstates: [4]
    undocumented: false
  - encoding: 54
    mnemonic: LD D, H
    operands: { R0: D, R1: H }
    tstates: [4]
    undocumented: false
  - encoding: 55
    mnemonic: LD D, L
    operands: { R0: D, R1: L }
    tstates: [4]
    undocumented: false
  - encoding: 57
    mnemonic: LD D, A
    operands: { R0: D, R1: A }
    tstates: [4]
    undocumented: false
  - encoding: 58
    mnemonic: LD E, B
    operands: { R0: E, R1: B }
    tstates: [4]
    undocumented: false
  - encoding: 59
    mnemonic: LD E, C
    operands: { R0: E, R1: C }
    tstates: [4]
    undocumented: false
  - encoding: 5A
    mnemonic: LD E, D
    operands: { R0: E, R1: D }
    tstates: [4]
    undocumented: false
  - encoding: 5B
    mnemonic: LD E, E
    operands: { R0: E, R1: E }
    tstates: [4]
    undocumented: false
  - encoding: 5C
    mnemonic: LD E, H
    operands: { R0: E, R1: H }
    tstates: [4]
    undocumented: false
  - encoding: 5D
    mnemonic: LD E, L
    operands: { R0: E, R1: L }
    tstates: [4]
    undocumented: false
  - encoding: 5F
    mnemonic: LD E, A
    operands: { R0: E, R1: A }
    tstates: [4]
    undocumented: false
  - encoding: 60
    mnemonic: LD H, B
    operands: { R0: H, R1: B }
    tstates: [4]
    undocumented: false
  - encoding: 61
    mnemonic: LD H, C
    operands: { R0: H, R1: C }
    tstates: [4]
    undocumented: false
  - encoding: 62
    mnemonic: LD H, D
    operands: { R0: H, R1: D }
    tstates: [4]
    undocumented: false
  - encoding: 63
    mnemonic: LD H, E
    operands: { R0: H, R1: E }
    tstates: [4]
    undocumented: false
  - encoding: 64
    mnemonic: LD H, H
    operands: { R0: H, R1: H }
    tstates: [4]
    undocumented: false
  - encoding: 65
    mnemonic: LD H, L
    operands: { R0: H, R1: L }
    tstates: [4]
    undocumented: false
  - encoding: 67
    mnemonic: LD H, A
    operands: { R0: H, R1: A }
    tstates: [4]
    undocumented: false
  - encoding: 68
    mnemonic: LD L, B
    operands: { R0: L, R1: B }
    tstates: [4]
    undocumented: false
  - encoding: 69
    mnemonic: LD L, C
    operands: { R0: L, R1: C }
    tstates: [4]
    undocumented: false
  - encoding: 6A
    mnemonic: LD L, D
    operands: { R0: L, R1: D }
    tstates: [4]
    undocumented: false
  - encoding: 6B
    mnemonic: LD L, E
    operands: { R0: L, R1: E }
    tstates: [4]
    undocumented: false
  - encoding: 6C
    mnemonic: LD L, H
    operands: { R0: L, R1: H }
    tstates: [4]
    undocumented: false
  - encoding: 6D
    mnemonic: LD L, L
    operands: { R0: L, R1: L }
    tstates: [4]
    undocumented: false
  - encoding: 6F
    mnemonic: LD L, A
    operands: { R0: L, R1: A }
    tstates: [4]
    undocumented: false
  - encoding: 78
    mnemonic: LD A, B
    operands: { R0: A, R1: B }
    tstates: [4]
    undocumented: false
  - encoding: 79
    mnemonic: LD A, C
    operands: { R0: A, R1: C }
    tstates: [4]
    undocumented: false
  - encoding: 7A
    mnemonic: LD A, D
    operands: { R0: A, R1: D }
    tstates: [4]
    undocumented: false
  - encoding: 7B
    mnemonic: LD A, E
    operands: { R0: A, R1: E }
    tstates: [4]
    undocumented: false
  - encoding: 7C
    mnemonic: LD A, H
    operands: { R0: A, R1: H }
    tstates: [4]
    undocumented: false
  - encoding: 7D
    mnemonic: LD A, L
    operands: { R0: A, R1: L }
    tstates: [4]
    undocumented: false
  - encoding: 7F
    mnemonic: LD A, A
    operands: { R0: A, R1: A }
    tstates: [4]
    undocumented: false
  - encoding: DD 40
    mnemonic: LD B, B
    operands: { R0: B, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 41
    mnemonic: LD B, C
    operands: { R0: B, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 42
    mnemonic: LD B, D
    operands: { R0: B, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 43
    mnemonic: LD B, E
    operands: { R0: B, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 44
    mnemonic: LD B, IXH
    operands: { R0: B, R1: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 45
    mnemonic: LD B, IXL
    operands: { R0: B, R1: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 47
    mnemonic: LD B, A
    operands: { R0: B, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: DD 48
    mnemonic: LD C, B
    operands: { R0: C, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 49
    mnemonic: LD C, C
    operands: { R0: C, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 4A
    mnemonic: LD C, D
    operands: { R0: C, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 4B
    mnemonic: LD C, E
    operands: { R0: C, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 4C
    mnemonic: LD C, IXH
    operands: { R0: C, R1: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 4D
    mnemonic: LD C, IXL
    operands: { R0: C, R1: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 4F
    mnemonic: LD C, A
    operands: { R0: C, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: DD 50
    mnemonic: LD D, B
    operands: { R0: D, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 51
    mnemonic: LD D, C
    operands: { R0: D, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 52
    mnemonic: LD D, D
    operands: { R0: D, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 53
    mnemonic: LD D, E
    operands: { R0: D, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 54
    mnemonic: LD D, IXH
    operands: { R0: D, R1: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 55
    mnemonic: LD D, IXL
    operands: { R0: D, R1: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 57
    mnemonic: LD D, A
    operands: { R0: D, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: DD 58
    mnemonic: LD E, B
    operands: { R0: E, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 59
    mnemonic: LD E, C
    operands: { R0: E, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 5A
    mnemonic: LD E, D
    operands: { R0: E, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 5B
    mnemonic: LD E, E
    operands: { R0: E, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 5C
    mnemonic: LD E, IXH
    operands: { R0: E, R1: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 5D
    mnemonic: LD E, IXL
    operands: { R0: E, R1: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 5F
    mnemonic: LD E, A
    operands: { R0: E, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: DD 60
    mnemonic: LD IXH, B
    operands: { R0: IXH, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 61
    mnemonic: LD IXH, C
    operands: { R0: IXH, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 62
    mnemonic: LD IXH, D
    operands: { R0: IXH, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 63
    mnemonic: LD IXH, E
    operands: { R0: IXH, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 64
    mnemonic: LD IXH, IXH
    operands: { R0: IXH, R1: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 65
    mnemonic: LD IXH, IXL
    operands: { R0: IXH, R1: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 67
    mnemonic: LD IXH, A
    operands: { R0: IXH, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: DD 68
    mnemonic: LD IXL, B
    operands: { R0: IXL, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 69
    mnemonic: LD IXL, C
    operands: { R0: IXL, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 6A
    mnemonic: LD IXL, D
    operands: { R0: IXL, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 6B
    mnemonic: LD IXL, E
    operands: { R0: IXL, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 6C
    mnemonic: LD IXL, IXH
    operands: { R0: IXL, R1: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 6D
    mnemonic: LD IXL, IXL
    operands: { R0: IXL, R1: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 6F
    mnemonic: LD IXL, A
    operands: { R0: IXL, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: DD 78
    mnemonic: LD A, B
    operands: { R0: A, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: DD 79
    mnemonic: LD A, C
    operands: { R0: A, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: DD 7A
    mnemonic: LD A, D
    operands: { R0: A, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: DD 7B
    mnemonic: LD A, E
    operands: { R0: A, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: DD 7C
    mnemonic: LD A, IXH
    operands: { R0: A, R1: IXH }
    tstates: [8]
    undocumented: true
  - encoding: DD 7D
    mnemonic: LD A, IXL
    operands: { R0: A, R1: IXL }
    tstates: [8]
    undocumented: true
  - encoding: DD 7F
    mnemonic: LD A, A
    operands: { R0: A, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 40
    mnemonic: LD B, B
    operands: { R0: B, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 41
    mnemonic: LD B, C
    operands: { R0: B, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 42
    mnemonic: LD B, D
    operands: { R0: B, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 43
    mnemonic: LD B, E
    operands: { R0: B, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 44
    mnemonic: LD B, IYH
    operands: { R0: B, R1: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 45
    mnemonic: LD B, IYL
    operands: { R0: B, R1: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 47
    mnemonic: LD B, A
    operands: { R0: B, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 48
    mnemonic: LD C, B
    operands: { R0: C, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 49
    mnemonic: LD C, C
    operands: { R0: C, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 4A
    mnemonic: LD C, D
    operands: { R0: C, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 4B
    mnemonic: LD C, E
    operands: { R0: C, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 4C
    mnemonic: LD C, IYH
    operands: { R0: C, R1: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 4D
    mnemonic: LD C, IYL
    operands: { R0: C, R1: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 4F
    mnemonic: LD C, A
    operands: { R0: C, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 50
    mnemonic: LD D, B
    operands: { R0: D, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 51
    mnemonic: LD D, C
    operands: { R0: D, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 52
    mnemonic: LD D, D
    operands: { R0: D, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 53
    mnemonic: LD D, E
    operands: { R0: D, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 54
    mnemonic: LD D, IYH
    operands: { R0: D, R1: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 55
    mnemonic: LD D, IYL
    operands: { R0: D, R1: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 57
    mnemonic: LD D, A
    operands: { R0: D, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 58
    mnemonic: LD E, B
    operands: { R0: E, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 59
    mnemonic: LD E, C
    operands: { R0: E, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 5A
    mnemonic: LD E, D
    operands: { R0: E, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 5B
    mnemonic: LD E, E
    operands: { R0: E, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 5C
    mnemonic: LD E, IYH
    operands: { R0: E, R1: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 5D
    mnemonic: LD E, IYL
    operands: { R0: E, R1: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 5F
    mnemonic: LD E, A
    operands: { R0: E, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 60
    mnemonic: LD IYH, B
    operands: { R0: IYH, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 61
    mnemonic: LD IYH, C
    operands: { R0: IYH, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 62
    mnemonic: LD IYH, D
    operands: { R0: IYH, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 63
    mnemonic: LD IYH, E
    operands: { R0: IYH, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 64
    mnemonic: LD IYH, IYH
    operands: { R0: IYH, R1: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 65
    mnemonic: LD IYH, IYL
    operands: { R0: IYH, R1: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 67
    mnemonic: LD IYH, A
    operands: { R0: IYH, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 68
    mnemonic: LD IYL, B
    operands: { R0: IYL, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 69
    mnemonic: LD IYL, C
    operands: { R0: IYL, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 6A
    mnemonic: LD IYL, D
    operands: { R0: IYL, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 6B
    mnemonic: LD IYL, E
    operands: { R0: IYL, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 6C
    mnemonic: LD IYL, IYH
    operands: { R0: IYL, R1: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 6D
    mnemonic: LD IYL, IYL
    operands: { R0: IYL, R1: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 6F
    mnemonic: LD IYL, A
    operands: { R0: IYL, R1: A }
    tstates: [8]
    undocumented: true
  - encoding: FD 78
    mnemonic: LD A, B
    operands: { R0: A, R1: B }
    tstates: [8]
    undocumented: true
  - encoding: FD 79
    mnemonic: LD A, C
    operands: { R0: A, R1: C }
    tstates: [8]
    undocumented: true
  - encoding: FD 7A
    mnemonic: LD A, D
    operands: { R0: A, R1: D }
    tstates: [8]
    undocumented: true
  - encoding: FD 7B
    mnemonic: LD A, E
    operands: { R0: A, R1: E }
    tstates: [8]
    undocumented: true
  - encoding: FD 7C
    mnemonic: LD A, IYH
    operands: { R0: A, R1: IYH }
    tstates: [8]
    undocumented: true
  - encoding: FD 7D
    mnemonic: LD A, IYL
    operands: { R0: A, R1: IYL }
    tstates: [8]
    undocumented: true
  - encoding: FD 7F
    mnemonic: LD A, A
    operands: { R0: A, R1: A }
    tstates: [8]
    undocumented: true
references:
  - name: Zilog Z80 CPU User Manual
    url: "https://www.zilog.com/docs/z80/um0080.pdf"
  - name: Z80 instruction set, MSX Assembly Page
    url: "https://map.grauw.nl/resources/z80instr.php"
---

# LD r, r'

Copies r' into r.

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
| `LD B, B` | `40` | 4 |  |
| `LD B, C` | `41` | 4 |  |
| `LD B, D` | `42` | 4 |  |
| `LD B, E` | `43` | 4 |  |
| `LD B, H` | `44` | 4 |  |
| `LD B, L` | `45` | 4 |  |
| `LD B, A` | `47` | 4 |  |
| `LD C, B` | `48` | 4 |  |
| `LD C, C` | `49` | 4 |  |
| `LD C, D` | `4A` | 4 |  |
| `LD C, E` | `4B` | 4 |  |
| `LD C, H` | `4C` | 4 |  |
| `LD C, L` | `4D` | 4 |  |
| `LD C, A` | `4F` | 4 |  |
| `LD D, B` | `50` | 4 |  |
| `LD D, C` | `51` | 4 |  |
| `LD D, D` | `52` | 4 |  |
| `LD D, E` | `53` | 4 |  |
| `LD D, H` | `54` | 4 |  |
| `LD D, L` | `55` | 4 |  |
| `LD D, A` | `57` | 4 |  |
| `LD E, B` | `58` | 4 |  |
| `LD E, C` | `59` | 4 |  |
| `LD E, D` | `5A` | 4 |  |
| `LD E, E` | `5B` | 4 |  |
| `LD E, H` | `5C` | 4 |  |
| `LD E, L` | `5D` | 4 |  |
| `LD E, A` | `5F` | 4 |  |
| `LD H, B` | `60` | 4 |  |
| `LD H, C` | `61` | 4 |  |
| `LD H, D` | `62` | 4 |  |
| `LD H, E` | `63` | 4 |  |
| `LD H, H` | `64` | 4 |  |
| `LD H, L` | `65` | 4 |  |
| `LD H, A` | `67` | 4 |  |
| `LD L, B` | `68` | 4 |  |
| `LD L, C` | `69` | 4 |  |
| `LD L, D` | `6A` | 4 |  |
| `LD L, E` | `6B` | 4 |  |
| `LD L, H` | `6C` | 4 |  |
| `LD L, L` | `6D` | 4 |  |
| `LD L, A` | `6F` | 4 |  |
| `LD A, B` | `78` | 4 |  |
| `LD A, C` | `79` | 4 |  |
| `LD A, D` | `7A` | 4 |  |
| `LD A, E` | `7B` | 4 |  |
| `LD A, H` | `7C` | 4 |  |
| `LD A, L` | `7D` | 4 |  |
| `LD A, A` | `7F` | 4 |  |
| `LD B, B` | `DD 40` | 8 | Undocumented. |
| `LD B, C` | `DD 41` | 8 | Undocumented. |
| `LD B, D` | `DD 42` | 8 | Undocumented. |
| `LD B, E` | `DD 43` | 8 | Undocumented. |
| `LD B, IXH` | `DD 44` | 8 | Undocumented. |
| `LD B, IXL` | `DD 45` | 8 | Undocumented. |
| `LD B, A` | `DD 47` | 8 | Undocumented. |
| `LD C, B` | `DD 48` | 8 | Undocumented. |
| `LD C, C` | `DD 49` | 8 | Undocumented. |
| `LD C, D` | `DD 4A` | 8 | Undocumented. |
| `LD C, E` | `DD 4B` | 8 | Undocumented. |
| `LD C, IXH` | `DD 4C` | 8 | Undocumented. |
| `LD C, IXL` | `DD 4D` | 8 | Undocumented. |
| `LD C, A` | `DD 4F` | 8 | Undocumented. |
| `LD D, B` | `DD 50` | 8 | Undocumented. |
| `LD D, C` | `DD 51` | 8 | Undocumented. |
| `LD D, D` | `DD 52` | 8 | Undocumented. |
| `LD D, E` | `DD 53` | 8 | Undocumented. |
| `LD D, IXH` | `DD 54` | 8 | Undocumented. |
| `LD D, IXL` | `DD 55` | 8 | Undocumented. |
| `LD D, A` | `DD 57` | 8 | Undocumented. |
| `LD E, B` | `DD 58` | 8 | Undocumented. |
| `LD E, C` | `DD 59` | 8 | Undocumented. |
| `LD E, D` | `DD 5A` | 8 | Undocumented. |
| `LD E, E` | `DD 5B` | 8 | Undocumented. |
| `LD E, IXH` | `DD 5C` | 8 | Undocumented. |
| `LD E, IXL` | `DD 5D` | 8 | Undocumented. |
| `LD E, A` | `DD 5F` | 8 | Undocumented. |
| `LD IXH, B` | `DD 60` | 8 | Undocumented. |
| `LD IXH, C` | `DD 61` | 8 | Undocumented. |
| `LD IXH, D` | `DD 62` | 8 | Undocumented. |
| `LD IXH, E` | `DD 63` | 8 | Undocumented. |
| `LD IXH, IXH` | `DD 64` | 8 | Undocumented. |
| `LD IXH, IXL` | `DD 65` | 8 | Undocumented. |
| `LD IXH, A` | `DD 67` | 8 | Undocumented. |
| `LD IXL, B` | `DD 68` | 8 | Undocumented. |
| `LD IXL, C` | `DD 69` | 8 | Undocumented. |
| `LD IXL, D` | `DD 6A` | 8 | Undocumented. |
| `LD IXL, E` | `DD 6B` | 8 | Undocumented. |
| `LD IXL, IXH` | `DD 6C` | 8 | Undocumented. |
| `LD IXL, IXL` | `DD 6D` | 8 | Undocumented. |
| `LD IXL, A` | `DD 6F` | 8 | Undocumented. |
| `LD A, B` | `DD 78` | 8 | Undocumented. |
| `LD A, C` | `DD 79` | 8 | Undocumented. |
| `LD A, D` | `DD 7A` | 8 | Undocumented. |
| `LD A, E` | `DD 7B` | 8 | Undocumented. |
| `LD A, IXH` | `DD 7C` | 8 | Undocumented. |
| `LD A, IXL` | `DD 7D` | 8 | Undocumented. |
| `LD A, A` | `DD 7F` | 8 | Undocumented. |
| `LD B, B` | `FD 40` | 8 | Undocumented. |
| `LD B, C` | `FD 41` | 8 | Undocumented. |
| `LD B, D` | `FD 42` | 8 | Undocumented. |
| `LD B, E` | `FD 43` | 8 | Undocumented. |
| `LD B, IYH` | `FD 44` | 8 | Undocumented. |
| `LD B, IYL` | `FD 45` | 8 | Undocumented. |
| `LD B, A` | `FD 47` | 8 | Undocumented. |
| `LD C, B` | `FD 48` | 8 | Undocumented. |
| `LD C, C` | `FD 49` | 8 | Undocumented. |
| `LD C, D` | `FD 4A` | 8 | Undocumented. |
| `LD C, E` | `FD 4B` | 8 | Undocumented. |
| `LD C, IYH` | `FD 4C` | 8 | Undocumented. |
| `LD C, IYL` | `FD 4D` | 8 | Undocumented. |
| `LD C, A` | `FD 4F` | 8 | Undocumented. |
| `LD D, B` | `FD 50` | 8 | Undocumented. |
| `LD D, C` | `FD 51` | 8 | Undocumented. |
| `LD D, D` | `FD 52` | 8 | Undocumented. |
| `LD D, E` | `FD 53` | 8 | Undocumented. |
| `LD D, IYH` | `FD 54` | 8 | Undocumented. |
| `LD D, IYL` | `FD 55` | 8 | Undocumented. |
| `LD D, A` | `FD 57` | 8 | Undocumented. |
| `LD E, B` | `FD 58` | 8 | Undocumented. |
| `LD E, C` | `FD 59` | 8 | Undocumented. |
| `LD E, D` | `FD 5A` | 8 | Undocumented. |
| `LD E, E` | `FD 5B` | 8 | Undocumented. |
| `LD E, IYH` | `FD 5C` | 8 | Undocumented. |
| `LD E, IYL` | `FD 5D` | 8 | Undocumented. |
| `LD E, A` | `FD 5F` | 8 | Undocumented. |
| `LD IYH, B` | `FD 60` | 8 | Undocumented. |
| `LD IYH, C` | `FD 61` | 8 | Undocumented. |
| `LD IYH, D` | `FD 62` | 8 | Undocumented. |
| `LD IYH, E` | `FD 63` | 8 | Undocumented. |
| `LD IYH, IYH` | `FD 64` | 8 | Undocumented. |
| `LD IYH, IYL` | `FD 65` | 8 | Undocumented. |
| `LD IYH, A` | `FD 67` | 8 | Undocumented. |
| `LD IYL, B` | `FD 68` | 8 | Undocumented. |
| `LD IYL, C` | `FD 69` | 8 | Undocumented. |
| `LD IYL, D` | `FD 6A` | 8 | Undocumented. |
| `LD IYL, E` | `FD 6B` | 8 | Undocumented. |
| `LD IYL, IYH` | `FD 6C` | 8 | Undocumented. |
| `LD IYL, IYL` | `FD 6D` | 8 | Undocumented. |
| `LD IYL, A` | `FD 6F` | 8 | Undocumented. |
| `LD A, B` | `FD 78` | 8 | Undocumented. |
| `LD A, C` | `FD 79` | 8 | Undocumented. |
| `LD A, D` | `FD 7A` | 8 | Undocumented. |
| `LD A, E` | `FD 7B` | 8 | Undocumented. |
| `LD A, IYH` | `FD 7C` | 8 | Undocumented. |
| `LD A, IYL` | `FD 7D` | 8 | Undocumented. |
| `LD A, A` | `FD 7F` | 8 | Undocumented. |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
