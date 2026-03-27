# Goal

* Create a code generator that builds a second emulator that has a granularity of instruction level rather than the step level in Z80Emulator.
* Must be faster than the step level emulator; it will be used in training RL models. More speed = faster training.
* Supporting contention is a nice to have but we can live without it.

# Spec

* Code generation to build a new Z80InstructionEmulator.
* Totally separate from the existing Z80Emulator. Code for Z80Emulator and the Contention classes should not change.
* Same external properies as Z80Emulator, i.e. Registers, Interrupts, etc. The types for these properties can be shared.
* One `ìnt ExecuteInstruction(Action<ActionRequired, ushort, byte)` method that will be called in a loop to run the emulation. Its behaviour will be similar to the hand-rolled ExecuteInstruction in the steppable emulator.
* ExecuteInstruction should:
   * Perform the opcode read cycle to determine the instruction
   * Perform all steps for the instruction, include any overlapped steps.
   * Test for interrupts and perform the interrupt handler if required.
   * Return the emulator to a state where the next call will execute the next instruction.
   * Return the number of T-states executed by the instruction. (Including T-states for interrupt handler)
* There should be no internal looping or separate step methods when executing an instruction. The flow is:
   * Perform opcode read to determine the instruction.
   * If it returns a prefix, perform opcode read again to fully determine the instruction.
   * Execute the method that performs an instruction in its entirety. This includes overlaps steps.
   * Perform a post execution method that checks interrupts, etc.
* ExecuteInstruction should take an `Action<ActionRequired, ushort, byte>` that can be called to perform IO. This is called with the action, the address and the data values.
* It generates from the same set of YAML files as the Z80Emulator. Do not change these.
* It should share a lot of the existing code generation. E.g. most of the steps, flags, etc will all be the same. We just need to put it in one method instead of many.

# Testing

* All tests should be ran in release mode for speed.
* Get the test suites to pass one at a time. One test suite should pass completely before moving to the next. Order to make them pass:
   * Fuse
   * SingleStep
   * Raxoft - Full and Memptr only
   * MarkWoodmass
   * ZEXALL - ZEXALL only, not ZEXDOC
* Interrupt tests are steppable only so can be ignored. We will need new instruction level interrupt tests.

# Current Implementation

* It has many issues including:
   * Separate overlap handling. This should be folded into the code for executing an instruction and not called separately.
   * Instruction methods perform loops; they shouldn't need to, just execute the code for the steps sequentially.
   * Lots of dead code such as empty do/while loops.
* Feel free to scrap the existing implementation and start again.
* Also the generator code is no longer emulator agnostic, e.g. there are fields such as PrefixCBStepFieldName. CB as a prefix is specific to the Z80 - other chips would have different prefixes or none at all.