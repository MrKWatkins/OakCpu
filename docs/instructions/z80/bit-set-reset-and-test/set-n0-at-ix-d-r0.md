# SET b, (IX + d), r

Sets bit b of the byte at (IX + d), storing the result in both the byte at (IX + d) and r.

This instruction is undocumented.

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

| Instruction | Opcode | T-States | Notes |
| ----------- | ------ | -------- | ----- |
| `SET 0, (IX + d), B` | `DD CB d C0` | 23 |  |
| `SET 0, (IX + d), C` | `DD CB d C1` | 23 |  |
| `SET 0, (IX + d), D` | `DD CB d C2` | 23 |  |
| `SET 0, (IX + d), E` | `DD CB d C3` | 23 |  |
| `SET 0, (IX + d), H` | `DD CB d C4` | 23 |  |
| `SET 0, (IX + d), L` | `DD CB d C5` | 23 |  |
| `SET 0, (IX + d), A` | `DD CB d C7` | 23 |  |
| `SET 1, (IX + d), B` | `DD CB d C8` | 23 |  |
| `SET 1, (IX + d), C` | `DD CB d C9` | 23 |  |
| `SET 1, (IX + d), D` | `DD CB d CA` | 23 |  |
| `SET 1, (IX + d), E` | `DD CB d CB` | 23 |  |
| `SET 1, (IX + d), H` | `DD CB d CC` | 23 |  |
| `SET 1, (IX + d), L` | `DD CB d CD` | 23 |  |
| `SET 1, (IX + d), A` | `DD CB d CF` | 23 |  |
| `SET 2, (IX + d), B` | `DD CB d D0` | 23 |  |
| `SET 2, (IX + d), C` | `DD CB d D1` | 23 |  |
| `SET 2, (IX + d), D` | `DD CB d D2` | 23 |  |
| `SET 2, (IX + d), E` | `DD CB d D3` | 23 |  |
| `SET 2, (IX + d), H` | `DD CB d D4` | 23 |  |
| `SET 2, (IX + d), L` | `DD CB d D5` | 23 |  |
| `SET 2, (IX + d), A` | `DD CB d D7` | 23 |  |
| `SET 3, (IX + d), B` | `DD CB d D8` | 23 |  |
| `SET 3, (IX + d), C` | `DD CB d D9` | 23 |  |
| `SET 3, (IX + d), D` | `DD CB d DA` | 23 |  |
| `SET 3, (IX + d), E` | `DD CB d DB` | 23 |  |
| `SET 3, (IX + d), H` | `DD CB d DC` | 23 |  |
| `SET 3, (IX + d), L` | `DD CB d DD` | 23 |  |
| `SET 3, (IX + d), A` | `DD CB d DF` | 23 |  |
| `SET 4, (IX + d), B` | `DD CB d E0` | 23 |  |
| `SET 4, (IX + d), C` | `DD CB d E1` | 23 |  |
| `SET 4, (IX + d), D` | `DD CB d E2` | 23 |  |
| `SET 4, (IX + d), E` | `DD CB d E3` | 23 |  |
| `SET 4, (IX + d), H` | `DD CB d E4` | 23 |  |
| `SET 4, (IX + d), L` | `DD CB d E5` | 23 |  |
| `SET 4, (IX + d), A` | `DD CB d E7` | 23 |  |
| `SET 5, (IX + d), B` | `DD CB d E8` | 23 |  |
| `SET 5, (IX + d), C` | `DD CB d E9` | 23 |  |
| `SET 5, (IX + d), D` | `DD CB d EA` | 23 |  |
| `SET 5, (IX + d), E` | `DD CB d EB` | 23 |  |
| `SET 5, (IX + d), H` | `DD CB d EC` | 23 |  |
| `SET 5, (IX + d), L` | `DD CB d ED` | 23 |  |
| `SET 5, (IX + d), A` | `DD CB d EF` | 23 |  |
| `SET 6, (IX + d), B` | `DD CB d F0` | 23 |  |
| `SET 6, (IX + d), C` | `DD CB d F1` | 23 |  |
| `SET 6, (IX + d), D` | `DD CB d F2` | 23 |  |
| `SET 6, (IX + d), E` | `DD CB d F3` | 23 |  |
| `SET 6, (IX + d), H` | `DD CB d F4` | 23 |  |
| `SET 6, (IX + d), L` | `DD CB d F5` | 23 |  |
| `SET 6, (IX + d), A` | `DD CB d F7` | 23 |  |
| `SET 7, (IX + d), B` | `DD CB d F8` | 23 |  |
| `SET 7, (IX + d), C` | `DD CB d F9` | 23 |  |
| `SET 7, (IX + d), D` | `DD CB d FA` | 23 |  |
| `SET 7, (IX + d), E` | `DD CB d FB` | 23 |  |
| `SET 7, (IX + d), H` | `DD CB d FC` | 23 |  |
| `SET 7, (IX + d), L` | `DD CB d FD` | 23 |  |
| `SET 7, (IX + d), A` | `DD CB d FF` | 23 |  |

## References

- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
- [The Undocumented Z80 Documented](http://www.z80.info/zip/z80-documented.pdf)
