# Implementation Details

## Optimizations Tried

I tried the following optimizations, none of which produced a noticeable performance improvement:

* For flags statements I tried moving them into methods if they were duplicated, the idea being to reduce code size and hoping the cache locality would outweigh the extra call.
* Move the instruction complete into their own methods, one for each combination of whether the instruction updates flags or not, and whether the opcode read is overlapped. Again trying to reduce code size.
* As above but also with the interrupt handler inlined into the instruction complete methods.