# 6502 Instructions

The instructions by group. See [the opcode tables](opcodes.md) for the instructions by opcode.

## Arithmetic

- [ADC #n](arithmetic/adc-n.md): Adds n and the carry flag to A. In decimal mode the addition is binary coded decimal.
- [ADC (zp),Y](arithmetic/adc-at-zp-y.md): Adds the byte at Y plus the address stored at zero page address zp and the carry flag to A. In decimal mode the addition is binary coded decimal. Takes an extra cycle if adding Y crosses a page.
- [ADC (zp,X)](arithmetic/adc-at-zp-x.md): Adds the byte at the address stored at zero page address zp + X and the carry flag to A. In decimal mode the addition is binary coded decimal. The zero page address wraps within page zero.
- [ADC abs](arithmetic/adc-abs.md): Adds the byte at address abs and the carry flag to A. In decimal mode the addition is binary coded decimal.
- [ADC abs,X](arithmetic/adc-abs-x.md): Adds the byte at address abs + X and the carry flag to A. In decimal mode the addition is binary coded decimal. Takes an extra cycle if adding X crosses a page.
- [ADC abs,Y](arithmetic/adc-abs-y.md): Adds the byte at address abs + Y and the carry flag to A. In decimal mode the addition is binary coded decimal. Takes an extra cycle if adding Y crosses a page.
- [ADC zp](arithmetic/adc-zp.md): Adds the byte at zero page address zp and the carry flag to A. In decimal mode the addition is binary coded decimal.
- [ADC zp,X](arithmetic/adc-zp-x.md): Adds the byte at zero page address zp + X and the carry flag to A. In decimal mode the addition is binary coded decimal. The zero page address wraps within page zero.
- [SBC #n](arithmetic/sbc-n.md): Subtracts n and the borrow, the inverse of the carry flag, from A. In decimal mode the subtraction is binary coded decimal.
- [SBC (zp),Y](arithmetic/sbc-at-zp-y.md): Subtracts the byte at Y plus the address stored at zero page address zp and the borrow, the inverse of the carry flag, from A. In decimal mode the subtraction is binary coded decimal. Takes an extra cycle if adding Y crosses a page.
- [SBC (zp,X)](arithmetic/sbc-at-zp-x.md): Subtracts the byte at the address stored at zero page address zp + X and the borrow, the inverse of the carry flag, from A. In decimal mode the subtraction is binary coded decimal. The zero page address wraps within page zero.
- [SBC abs](arithmetic/sbc-abs.md): Subtracts the byte at address abs and the borrow, the inverse of the carry flag, from A. In decimal mode the subtraction is binary coded decimal.
- [SBC abs,X](arithmetic/sbc-abs-x.md): Subtracts the byte at address abs + X and the borrow, the inverse of the carry flag, from A. In decimal mode the subtraction is binary coded decimal. Takes an extra cycle if adding X crosses a page.
- [SBC abs,Y](arithmetic/sbc-abs-y.md): Subtracts the byte at address abs + Y and the borrow, the inverse of the carry flag, from A. In decimal mode the subtraction is binary coded decimal. Takes an extra cycle if adding Y crosses a page.
- [SBC zp](arithmetic/sbc-zp.md): Subtracts the byte at zero page address zp and the borrow, the inverse of the carry flag, from A. In decimal mode the subtraction is binary coded decimal.
- [SBC zp,X](arithmetic/sbc-zp-x.md): Subtracts the byte at zero page address zp + X and the borrow, the inverse of the carry flag, from A. In decimal mode the subtraction is binary coded decimal. The zero page address wraps within page zero.

## Branch

- [BCC rel](branch/bcc-rel.md): Branches to the address of the next instruction plus the signed displacement rel if the C flag is reset. Takes an extra cycle if the branch is taken, and another if it crosses a page.
- [BCS rel](branch/bcs-rel.md): Branches to the address of the next instruction plus the signed displacement rel if the C flag is set. Takes an extra cycle if the branch is taken, and another if it crosses a page.
- [BEQ rel](branch/beq-rel.md): Branches to the address of the next instruction plus the signed displacement rel if the Z flag is set. Takes an extra cycle if the branch is taken, and another if it crosses a page.
- [BMI rel](branch/bmi-rel.md): Branches to the address of the next instruction plus the signed displacement rel if the N flag is set. Takes an extra cycle if the branch is taken, and another if it crosses a page.
- [BNE rel](branch/bne-rel.md): Branches to the address of the next instruction plus the signed displacement rel if the Z flag is reset. Takes an extra cycle if the branch is taken, and another if it crosses a page.
- [BPL rel](branch/bpl-rel.md): Branches to the address of the next instruction plus the signed displacement rel if the N flag is reset. Takes an extra cycle if the branch is taken, and another if it crosses a page.
- [BVC rel](branch/bvc-rel.md): Branches to the address of the next instruction plus the signed displacement rel if the V flag is reset. Takes an extra cycle if the branch is taken, and another if it crosses a page.
- [BVS rel](branch/bvs-rel.md): Branches to the address of the next instruction plus the signed displacement rel if the V flag is set. Takes an extra cycle if the branch is taken, and another if it crosses a page.

## Call and Return

- [JSR abs](call-and-return/jsr-abs.md): Pushes the address of the last byte of the instruction onto the stack and jumps to address abs.
- [RTS](call-and-return/rts.md): Pops the return address pushed by JSR from the stack and jumps to the byte after it.

## Compare

- [CMP #n](compare/cmp-n.md): Compares A with n by subtracting the byte from A, setting the flags from the result and discarding it.
- [CMP (zp),Y](compare/cmp-at-zp-y.md): Compares A with the byte at Y plus the address stored at zero page address zp by subtracting the byte from A, setting the flags from the result and discarding it. Takes an extra cycle if adding Y crosses a page.
- [CMP (zp,X)](compare/cmp-at-zp-x.md): Compares A with the byte at the address stored at zero page address zp + X by subtracting the byte from A, setting the flags from the result and discarding it. The zero page address wraps within page zero.
- [CMP abs](compare/cmp-abs.md): Compares A with the byte at address abs by subtracting the byte from A, setting the flags from the result and discarding it.
- [CMP abs,X](compare/cmp-abs-x.md): Compares A with the byte at address abs + X by subtracting the byte from A, setting the flags from the result and discarding it. Takes an extra cycle if adding X crosses a page.
- [CMP abs,Y](compare/cmp-abs-y.md): Compares A with the byte at address abs + Y by subtracting the byte from A, setting the flags from the result and discarding it. Takes an extra cycle if adding Y crosses a page.
- [CMP zp](compare/cmp-zp.md): Compares A with the byte at zero page address zp by subtracting the byte from A, setting the flags from the result and discarding it.
- [CMP zp,X](compare/cmp-zp-x.md): Compares A with the byte at zero page address zp + X by subtracting the byte from A, setting the flags from the result and discarding it. The zero page address wraps within page zero.
- [CPX #n](compare/cpx-n.md): Compares X with n by subtracting the byte from X, setting the flags from the result and discarding it.
- [CPX abs](compare/cpx-abs.md): Compares X with the byte at address abs by subtracting the byte from X, setting the flags from the result and discarding it.
- [CPX zp](compare/cpx-zp.md): Compares X with the byte at zero page address zp by subtracting the byte from X, setting the flags from the result and discarding it.
- [CPY #n](compare/cpy-n.md): Compares Y with n by subtracting the byte from Y, setting the flags from the result and discarding it.
- [CPY abs](compare/cpy-abs.md): Compares Y with the byte at address abs by subtracting the byte from Y, setting the flags from the result and discarding it.
- [CPY zp](compare/cpy-zp.md): Compares Y with the byte at zero page address zp by subtracting the byte from Y, setting the flags from the result and discarding it.

## General Purpose

- [NOP](general-purpose/nop.md): Does nothing.
- *[NOP #n](general-purpose/nop-n.md)*: Does nothing; the operand byte is read and ignored.
- *[NOP abs](general-purpose/nop-abs.md)*: Does nothing; the byte at address abs is read and ignored.
- *[NOP abs,X](general-purpose/nop-abs-x.md)*: Does nothing; the byte at address abs + X is read and ignored. Takes an extra cycle if adding X crosses a page.
- *[NOP zp](general-purpose/nop-zp.md)*: Does nothing; the byte at zero page address zp is read and ignored.
- *[NOP zp,X](general-purpose/nop-zp-x.md)*: Does nothing; the byte at zero page address zp + X is read and ignored. The zero page address wraps within page zero.

## Increment/Decrement

- [DEC abs](increment-decrement/dec-abs.md): Decrements the byte at address abs.
- [DEC abs,X](increment-decrement/dec-abs-x.md): Decrements the byte at address abs + X.
- [DEC zp](increment-decrement/dec-zp.md): Decrements the byte at zero page address zp.
- [DEC zp,X](increment-decrement/dec-zp-x.md): Decrements the byte at zero page address zp + X. The zero page address wraps within page zero.
- [DEX](increment-decrement/dex.md): Decrements X.
- [DEY](increment-decrement/dey.md): Decrements Y.
- [INC abs](increment-decrement/inc-abs.md): Increments the byte at address abs.
- [INC abs,X](increment-decrement/inc-abs-x.md): Increments the byte at address abs + X.
- [INC zp](increment-decrement/inc-zp.md): Increments the byte at zero page address zp.
- [INC zp,X](increment-decrement/inc-zp-x.md): Increments the byte at zero page address zp + X. The zero page address wraps within page zero.
- [INX](increment-decrement/inx.md): Increments X.
- [INY](increment-decrement/iny.md): Increments Y.

## Jump

- [JMP (abs)](jump/jmp-at-abs.md): Jumps to the address stored at address abs. If abs is the last byte of a page, the high byte of the address is read from the first byte of the same page rather than the next one.
- [JMP abs](jump/jmp-abs.md): Jumps to address abs.

## Load

- [LDA #n](load/lda-n.md): Loads A with n.
- [LDA (zp),Y](load/lda-at-zp-y.md): Loads A with the byte at Y plus the address stored at zero page address zp. Takes an extra cycle if adding Y crosses a page.
- [LDA (zp,X)](load/lda-at-zp-x.md): Loads A with the byte at the address stored at zero page address zp + X. The zero page address wraps within page zero.
- [LDA abs](load/lda-abs.md): Loads A with the byte at address abs.
- [LDA abs,X](load/lda-abs-x.md): Loads A with the byte at address abs + X. Takes an extra cycle if adding X crosses a page.
- [LDA abs,Y](load/lda-abs-y.md): Loads A with the byte at address abs + Y. Takes an extra cycle if adding Y crosses a page.
- [LDA zp](load/lda-zp.md): Loads A with the byte at zero page address zp.
- [LDA zp,X](load/lda-zp-x.md): Loads A with the byte at zero page address zp + X. The zero page address wraps within page zero.
- [LDX #n](load/ldx-n.md): Loads X with n.
- [LDX abs](load/ldx-abs.md): Loads X with the byte at address abs.
- [LDX abs,Y](load/ldx-abs-y.md): Loads X with the byte at address abs + Y. Takes an extra cycle if adding Y crosses a page.
- [LDX zp](load/ldx-zp.md): Loads X with the byte at zero page address zp.
- [LDX zp,Y](load/ldx-zp-y.md): Loads X with the byte at zero page address zp + Y. The zero page address wraps within page zero.
- [LDY #n](load/ldy-n.md): Loads Y with n.
- [LDY abs](load/ldy-abs.md): Loads Y with the byte at address abs.
- [LDY abs,X](load/ldy-abs-x.md): Loads Y with the byte at address abs + X. Takes an extra cycle if adding X crosses a page.
- [LDY zp](load/ldy-zp.md): Loads Y with the byte at zero page address zp.
- [LDY zp,X](load/ldy-zp-x.md): Loads Y with the byte at zero page address zp + X. The zero page address wraps within page zero.

## Logical

- [AND #n](logical/and-n.md): Bitwise ANDs A with n.
- [AND (zp),Y](logical/and-at-zp-y.md): Bitwise ANDs A with the byte at Y plus the address stored at zero page address zp. Takes an extra cycle if adding Y crosses a page.
- [AND (zp,X)](logical/and-at-zp-x.md): Bitwise ANDs A with the byte at the address stored at zero page address zp + X. The zero page address wraps within page zero.
- [AND abs](logical/and-abs.md): Bitwise ANDs A with the byte at address abs.
- [AND abs,X](logical/and-abs-x.md): Bitwise ANDs A with the byte at address abs + X. Takes an extra cycle if adding X crosses a page.
- [AND abs,Y](logical/and-abs-y.md): Bitwise ANDs A with the byte at address abs + Y. Takes an extra cycle if adding Y crosses a page.
- [AND zp](logical/and-zp.md): Bitwise ANDs A with the byte at zero page address zp.
- [AND zp,X](logical/and-zp-x.md): Bitwise ANDs A with the byte at zero page address zp + X. The zero page address wraps within page zero.
- [BIT abs](logical/bit-abs.md): Tests A against the byte at address abs: the Z flag is set from A AND the byte, and the N and V flags are copied from bits 7 and 6 of the byte.
- [BIT zp](logical/bit-zp.md): Tests A against the byte at zero page address zp: the Z flag is set from A AND the byte, and the N and V flags are copied from bits 7 and 6 of the byte.
- [EOR #n](logical/eor-n.md): Bitwise exclusive ORs A with n.
- [EOR (zp),Y](logical/eor-at-zp-y.md): Bitwise exclusive ORs A with the byte at Y plus the address stored at zero page address zp. Takes an extra cycle if adding Y crosses a page.
- [EOR (zp,X)](logical/eor-at-zp-x.md): Bitwise exclusive ORs A with the byte at the address stored at zero page address zp + X. The zero page address wraps within page zero.
- [EOR abs](logical/eor-abs.md): Bitwise exclusive ORs A with the byte at address abs.
- [EOR abs,X](logical/eor-abs-x.md): Bitwise exclusive ORs A with the byte at address abs + X. Takes an extra cycle if adding X crosses a page.
- [EOR abs,Y](logical/eor-abs-y.md): Bitwise exclusive ORs A with the byte at address abs + Y. Takes an extra cycle if adding Y crosses a page.
- [EOR zp](logical/eor-zp.md): Bitwise exclusive ORs A with the byte at zero page address zp.
- [EOR zp,X](logical/eor-zp-x.md): Bitwise exclusive ORs A with the byte at zero page address zp + X. The zero page address wraps within page zero.
- [ORA #n](logical/ora-n.md): Bitwise ORs A with n.
- [ORA (zp),Y](logical/ora-at-zp-y.md): Bitwise ORs A with the byte at Y plus the address stored at zero page address zp. Takes an extra cycle if adding Y crosses a page.
- [ORA (zp,X)](logical/ora-at-zp-x.md): Bitwise ORs A with the byte at the address stored at zero page address zp + X. The zero page address wraps within page zero.
- [ORA abs](logical/ora-abs.md): Bitwise ORs A with the byte at address abs.
- [ORA abs,X](logical/ora-abs-x.md): Bitwise ORs A with the byte at address abs + X. Takes an extra cycle if adding X crosses a page.
- [ORA abs,Y](logical/ora-abs-y.md): Bitwise ORs A with the byte at address abs + Y. Takes an extra cycle if adding Y crosses a page.
- [ORA zp](logical/ora-zp.md): Bitwise ORs A with the byte at zero page address zp.
- [ORA zp,X](logical/ora-zp-x.md): Bitwise ORs A with the byte at zero page address zp + X. The zero page address wraps within page zero.

## Shift

- [ASL A](shift/asl-a.md): Shifts A left one bit, moving bit 7 into the carry flag and resetting bit 0.
- [ASL abs](shift/asl-abs.md): Shifts the byte at address abs left one bit, moving bit 7 into the carry flag and resetting bit 0.
- [ASL abs,X](shift/asl-abs-x.md): Shifts the byte at address abs + X left one bit, moving bit 7 into the carry flag and resetting bit 0.
- [ASL zp](shift/asl-zp.md): Shifts the byte at zero page address zp left one bit, moving bit 7 into the carry flag and resetting bit 0.
- [ASL zp,X](shift/asl-zp-x.md): Shifts the byte at zero page address zp + X left one bit, moving bit 7 into the carry flag and resetting bit 0. The zero page address wraps within page zero.
- [LSR A](shift/lsr-a.md): Shifts A right one bit, moving bit 0 into the carry flag and resetting bit 7.
- [LSR abs](shift/lsr-abs.md): Shifts the byte at address abs right one bit, moving bit 0 into the carry flag and resetting bit 7.
- [LSR abs,X](shift/lsr-abs-x.md): Shifts the byte at address abs + X right one bit, moving bit 0 into the carry flag and resetting bit 7.
- [LSR zp](shift/lsr-zp.md): Shifts the byte at zero page address zp right one bit, moving bit 0 into the carry flag and resetting bit 7.
- [LSR zp,X](shift/lsr-zp-x.md): Shifts the byte at zero page address zp + X right one bit, moving bit 0 into the carry flag and resetting bit 7. The zero page address wraps within page zero.
- [ROL A](shift/rol-a.md): Rotates A left one bit through the carry flag, moving bit 7 into the carry flag and the previous carry flag into bit 0.
- [ROL abs](shift/rol-abs.md): Rotates the byte at address abs left one bit through the carry flag, moving bit 7 into the carry flag and the previous carry flag into bit 0.
- [ROL abs,X](shift/rol-abs-x.md): Rotates the byte at address abs + X left one bit through the carry flag, moving bit 7 into the carry flag and the previous carry flag into bit 0.
- [ROL zp](shift/rol-zp.md): Rotates the byte at zero page address zp left one bit through the carry flag, moving bit 7 into the carry flag and the previous carry flag into bit 0.
- [ROL zp,X](shift/rol-zp-x.md): Rotates the byte at zero page address zp + X left one bit through the carry flag, moving bit 7 into the carry flag and the previous carry flag into bit 0. The zero page address wraps within page zero.
- [ROR A](shift/ror-a.md): Rotates A right one bit through the carry flag, moving bit 0 into the carry flag and the previous carry flag into bit 7.
- [ROR abs](shift/ror-abs.md): Rotates the byte at address abs right one bit through the carry flag, moving bit 0 into the carry flag and the previous carry flag into bit 7.
- [ROR abs,X](shift/ror-abs-x.md): Rotates the byte at address abs + X right one bit through the carry flag, moving bit 0 into the carry flag and the previous carry flag into bit 7.
- [ROR zp](shift/ror-zp.md): Rotates the byte at zero page address zp right one bit through the carry flag, moving bit 0 into the carry flag and the previous carry flag into bit 7.
- [ROR zp,X](shift/ror-zp-x.md): Rotates the byte at zero page address zp + X right one bit through the carry flag, moving bit 0 into the carry flag and the previous carry flag into bit 7. The zero page address wraps within page zero.

## Stack

- [PHA](stack/pha.md): Pushes A onto the stack.
- [PHP](stack/php.md): Pushes the processor status onto the stack with the B flag set.
- [PLA](stack/pla.md): Pops A from the stack.
- [PLP](stack/plp.md): Pops the processor status from the stack.

## Status

- [CLC](status/clc.md): Resets the carry flag.
- [CLD](status/cld.md): Resets the decimal flag, so that ADC and SBC work in binary.
- [CLI](status/cli.md): Resets the interrupt disable flag, enabling IRQ interrupts.
- [CLV](status/clv.md): Resets the overflow flag.
- [SEC](status/sec.md): Sets the carry flag.
- [SED](status/sed.md): Sets the decimal flag, so that ADC and SBC work in binary coded decimal.
- [SEI](status/sei.md): Sets the interrupt disable flag, disabling IRQ interrupts.

## Store

- [STA (zp),Y](store/sta-at-zp-y.md): Stores A at Y plus the address stored at zero page address zp.
- [STA (zp,X)](store/sta-at-zp-x.md): Stores A at the address stored at zero page address zp + X. The zero page address wraps within page zero.
- [STA abs](store/sta-abs.md): Stores A at address abs.
- [STA abs,X](store/sta-abs-x.md): Stores A at address abs + X.
- [STA abs,Y](store/sta-abs-y.md): Stores A at address abs + Y.
- [STA zp](store/sta-zp.md): Stores A at zero page address zp.
- [STA zp,X](store/sta-zp-x.md): Stores A at zero page address zp + X. The zero page address wraps within page zero.
- [STX abs](store/stx-abs.md): Stores X at address abs.
- [STX zp](store/stx-zp.md): Stores X at zero page address zp.
- [STX zp,Y](store/stx-zp-y.md): Stores X at zero page address zp + Y. The zero page address wraps within page zero.
- [STY abs](store/sty-abs.md): Stores Y at address abs.
- [STY zp](store/sty-zp.md): Stores Y at zero page address zp.
- [STY zp,X](store/sty-zp-x.md): Stores Y at zero page address zp + X. The zero page address wraps within page zero.

## System

- [BRK](system/brk.md): Pushes the address of the byte after the padding byte following the opcode, then the processor status with the B flag set, onto the stack, sets the interrupt disable flag and jumps to the address at the IRQ vector, 0xFFFE.
- [RTI](system/rti.md): Pops the processor status and then the return address from the stack, returning from an interrupt.

## Transfer

- [TAX](transfer/tax.md): Copies A into X.
- [TAY](transfer/tay.md): Copies A into Y.
- [TSX](transfer/tsx.md): Copies S into X.
- [TXA](transfer/txa.md): Copies X into A.
- [TXS](transfer/txs.md): Copies X into S.
- [TYA](transfer/tya.md): Copies Y into A.

## Undocumented

- *[AHX (zp),Y](undocumented/ahx-at-zp-y.md)*: Stores A AND X AND the high byte of the address stored at zp plus one at Y plus the address stored at zero page address zp. If adding Y crosses a page, the high byte of the address written is replaced by the value stored.
- *[AHX abs,Y](undocumented/ahx-abs-y.md)*: Stores A AND X AND the high byte of abs plus one at address abs + Y. If adding Y crosses a page, the high byte of the address written is replaced by the value stored.
- *[ALR #n](undocumented/alr-n.md)*: Bitwise ANDs A with n, then shifts the result right one bit, moving bit 0 into the carry flag.
- *[ANC #n](undocumented/anc-n.md)*: Bitwise ANDs A with n, then copies bit 7 of the result into the carry flag.
- *[ARR #n](undocumented/arr-n.md)*: Bitwise ANDs A with n, then rotates the result right one bit through the carry flag. The carry and overflow flags are set from bits 6 and 5 of the result as if by an addition, and in decimal mode the result is adjusted as it would be by ADC.
- *[DCP (zp),Y](undocumented/dcp-at-zp-y.md)*: Decrements the byte at Y plus the address stored at zero page address zp, then compares A with the result as CMP does.
- *[DCP (zp,X)](undocumented/dcp-at-zp-x.md)*: Decrements the byte at the address stored at zero page address zp + X, then compares A with the result as CMP does. The zero page address wraps within page zero.
- *[DCP abs](undocumented/dcp-abs.md)*: Decrements the byte at address abs, then compares A with the result as CMP does.
- *[DCP abs,X](undocumented/dcp-abs-x.md)*: Decrements the byte at address abs + X, then compares A with the result as CMP does.
- *[DCP abs,Y](undocumented/dcp-abs-y.md)*: Decrements the byte at address abs + Y, then compares A with the result as CMP does.
- *[DCP zp](undocumented/dcp-zp.md)*: Decrements the byte at zero page address zp, then compares A with the result as CMP does.
- *[DCP zp,X](undocumented/dcp-zp-x.md)*: Decrements the byte at zero page address zp + X, then compares A with the result as CMP does. The zero page address wraps within page zero.
- *[ISC (zp),Y](undocumented/isc-at-zp-y.md)*: Increments the byte at Y plus the address stored at zero page address zp, then subtracts the result and the borrow from A as SBC does.
- *[ISC (zp,X)](undocumented/isc-at-zp-x.md)*: Increments the byte at the address stored at zero page address zp + X, then subtracts the result and the borrow from A as SBC does. The zero page address wraps within page zero.
- *[ISC abs](undocumented/isc-abs.md)*: Increments the byte at address abs, then subtracts the result and the borrow from A as SBC does.
- *[ISC abs,X](undocumented/isc-abs-x.md)*: Increments the byte at address abs + X, then subtracts the result and the borrow from A as SBC does.
- *[ISC abs,Y](undocumented/isc-abs-y.md)*: Increments the byte at address abs + Y, then subtracts the result and the borrow from A as SBC does.
- *[ISC zp](undocumented/isc-zp.md)*: Increments the byte at zero page address zp, then subtracts the result and the borrow from A as SBC does.
- *[ISC zp,X](undocumented/isc-zp-x.md)*: Increments the byte at zero page address zp + X, then subtracts the result and the borrow from A as SBC does. The zero page address wraps within page zero.
- *[JAM](undocumented/jam.md)*: Jams the CPU: it reads from 0xFFFF and 0xFFFE and then does nothing until it is reset.
- *[LAS abs,Y](undocumented/las-abs-y.md)*: Loads A, X and S with the byte at address abs + Y AND S. Takes an extra cycle if adding Y crosses a page.
- *[LAX #n](undocumented/lax-n.md)*: Loads both A and X with n AND (A OR 0xEE). The 0xEE varies between chips, so the result is unreliable.
- *[LAX (zp),Y](undocumented/lax-at-zp-y.md)*: Loads both A and X with the byte at Y plus the address stored at zero page address zp. Takes an extra cycle if adding Y crosses a page.
- *[LAX (zp,X)](undocumented/lax-at-zp-x.md)*: Loads both A and X with the byte at the address stored at zero page address zp + X. The zero page address wraps within page zero.
- *[LAX abs](undocumented/lax-abs.md)*: Loads both A and X with the byte at address abs.
- *[LAX abs,Y](undocumented/lax-abs-y.md)*: Loads both A and X with the byte at address abs + Y. Takes an extra cycle if adding Y crosses a page.
- *[LAX zp](undocumented/lax-zp.md)*: Loads both A and X with the byte at zero page address zp.
- *[LAX zp,Y](undocumented/lax-zp-y.md)*: Loads both A and X with the byte at zero page address zp + Y. The zero page address wraps within page zero.
- *[RLA (zp),Y](undocumented/rla-at-zp-y.md)*: Rotates the byte at Y plus the address stored at zero page address zp left one bit through the carry flag, then bitwise ANDs A with the result.
- *[RLA (zp,X)](undocumented/rla-at-zp-x.md)*: Rotates the byte at the address stored at zero page address zp + X left one bit through the carry flag, then bitwise ANDs A with the result. The zero page address wraps within page zero.
- *[RLA abs](undocumented/rla-abs.md)*: Rotates the byte at address abs left one bit through the carry flag, then bitwise ANDs A with the result.
- *[RLA abs,X](undocumented/rla-abs-x.md)*: Rotates the byte at address abs + X left one bit through the carry flag, then bitwise ANDs A with the result.
- *[RLA abs,Y](undocumented/rla-abs-y.md)*: Rotates the byte at address abs + Y left one bit through the carry flag, then bitwise ANDs A with the result.
- *[RLA zp](undocumented/rla-zp.md)*: Rotates the byte at zero page address zp left one bit through the carry flag, then bitwise ANDs A with the result.
- *[RLA zp,X](undocumented/rla-zp-x.md)*: Rotates the byte at zero page address zp + X left one bit through the carry flag, then bitwise ANDs A with the result. The zero page address wraps within page zero.
- *[RRA (zp),Y](undocumented/rra-at-zp-y.md)*: Rotates the byte at Y plus the address stored at zero page address zp right one bit through the carry flag, then adds the result and the new carry flag to A as ADC does.
- *[RRA (zp,X)](undocumented/rra-at-zp-x.md)*: Rotates the byte at the address stored at zero page address zp + X right one bit through the carry flag, then adds the result and the new carry flag to A as ADC does. The zero page address wraps within page zero.
- *[RRA abs](undocumented/rra-abs.md)*: Rotates the byte at address abs right one bit through the carry flag, then adds the result and the new carry flag to A as ADC does.
- *[RRA abs,X](undocumented/rra-abs-x.md)*: Rotates the byte at address abs + X right one bit through the carry flag, then adds the result and the new carry flag to A as ADC does.
- *[RRA abs,Y](undocumented/rra-abs-y.md)*: Rotates the byte at address abs + Y right one bit through the carry flag, then adds the result and the new carry flag to A as ADC does.
- *[RRA zp](undocumented/rra-zp.md)*: Rotates the byte at zero page address zp right one bit through the carry flag, then adds the result and the new carry flag to A as ADC does.
- *[RRA zp,X](undocumented/rra-zp-x.md)*: Rotates the byte at zero page address zp + X right one bit through the carry flag, then adds the result and the new carry flag to A as ADC does. The zero page address wraps within page zero.
- *[SAX (zp,X)](undocumented/sax-at-zp-x.md)*: Stores A AND X at the address stored at zero page address zp + X. The zero page address wraps within page zero.
- *[SAX abs](undocumented/sax-abs.md)*: Stores A AND X at address abs.
- *[SAX zp](undocumented/sax-zp.md)*: Stores A AND X at zero page address zp.
- *[SAX zp,Y](undocumented/sax-zp-y.md)*: Stores A AND X at zero page address zp + Y. The zero page address wraps within page zero.
- *[SBX #n](undocumented/sbx-n.md)*: Subtracts n from A AND X, without the borrow, and stores the result in X.
- *[SHX abs,Y](undocumented/shx-abs-y.md)*: Stores X AND the high byte of abs plus one at address abs + Y. If adding Y crosses a page, the high byte of the address written is replaced by the value stored.
- *[SHY abs,X](undocumented/shy-abs-x.md)*: Stores Y AND the high byte of abs plus one at address abs + X. If adding X crosses a page, the high byte of the address written is replaced by the value stored.
- *[SLO (zp),Y](undocumented/slo-at-zp-y.md)*: Shifts the byte at Y plus the address stored at zero page address zp left one bit, moving bit 7 into the carry flag, then bitwise ORs A with the result.
- *[SLO (zp,X)](undocumented/slo-at-zp-x.md)*: Shifts the byte at the address stored at zero page address zp + X left one bit, moving bit 7 into the carry flag, then bitwise ORs A with the result. The zero page address wraps within page zero.
- *[SLO abs](undocumented/slo-abs.md)*: Shifts the byte at address abs left one bit, moving bit 7 into the carry flag, then bitwise ORs A with the result.
- *[SLO abs,X](undocumented/slo-abs-x.md)*: Shifts the byte at address abs + X left one bit, moving bit 7 into the carry flag, then bitwise ORs A with the result.
- *[SLO abs,Y](undocumented/slo-abs-y.md)*: Shifts the byte at address abs + Y left one bit, moving bit 7 into the carry flag, then bitwise ORs A with the result.
- *[SLO zp](undocumented/slo-zp.md)*: Shifts the byte at zero page address zp left one bit, moving bit 7 into the carry flag, then bitwise ORs A with the result.
- *[SLO zp,X](undocumented/slo-zp-x.md)*: Shifts the byte at zero page address zp + X left one bit, moving bit 7 into the carry flag, then bitwise ORs A with the result. The zero page address wraps within page zero.
- *[SRE (zp),Y](undocumented/sre-at-zp-y.md)*: Shifts the byte at Y plus the address stored at zero page address zp right one bit, moving bit 0 into the carry flag, then bitwise exclusive ORs A with the result.
- *[SRE (zp,X)](undocumented/sre-at-zp-x.md)*: Shifts the byte at the address stored at zero page address zp + X right one bit, moving bit 0 into the carry flag, then bitwise exclusive ORs A with the result. The zero page address wraps within page zero.
- *[SRE abs](undocumented/sre-abs.md)*: Shifts the byte at address abs right one bit, moving bit 0 into the carry flag, then bitwise exclusive ORs A with the result.
- *[SRE abs,X](undocumented/sre-abs-x.md)*: Shifts the byte at address abs + X right one bit, moving bit 0 into the carry flag, then bitwise exclusive ORs A with the result.
- *[SRE abs,Y](undocumented/sre-abs-y.md)*: Shifts the byte at address abs + Y right one bit, moving bit 0 into the carry flag, then bitwise exclusive ORs A with the result.
- *[SRE zp](undocumented/sre-zp.md)*: Shifts the byte at zero page address zp right one bit, moving bit 0 into the carry flag, then bitwise exclusive ORs A with the result.
- *[SRE zp,X](undocumented/sre-zp-x.md)*: Shifts the byte at zero page address zp + X right one bit, moving bit 0 into the carry flag, then bitwise exclusive ORs A with the result. The zero page address wraps within page zero.
- *[TAS abs,Y](undocumented/tas-abs-y.md)*: Stores A AND X in S, then stores S AND the high byte of abs plus one at address abs + Y. If adding Y crosses a page, the high byte of the address written is replaced by the value stored.
- *[USBC #n](undocumented/usbc-n.md)*: Subtracts n and the borrow, the inverse of the carry flag, from A, exactly as SBC #n does.
- *[XAA #n](undocumented/xaa-n.md)*: Loads A with (A OR 0xEE) AND X AND n. The 0xEE varies between chips, so the result is unreliable.
