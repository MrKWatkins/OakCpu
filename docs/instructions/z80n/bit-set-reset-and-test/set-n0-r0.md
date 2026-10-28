# SET b, r

Sets bit b of r.

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
| `SET 0, B` | `CB C0` | 8 |  |
| `SET 0, C` | `CB C1` | 8 |  |
| `SET 0, D` | `CB C2` | 8 |  |
| `SET 0, E` | `CB C3` | 8 |  |
| `SET 0, H` | `CB C4` | 8 |  |
| `SET 0, L` | `CB C5` | 8 |  |
| `SET 0, A` | `CB C7` | 8 |  |
| `SET 1, B` | `CB C8` | 8 |  |
| `SET 1, C` | `CB C9` | 8 |  |
| `SET 1, D` | `CB CA` | 8 |  |
| `SET 1, E` | `CB CB` | 8 |  |
| `SET 1, H` | `CB CC` | 8 |  |
| `SET 1, L` | `CB CD` | 8 |  |
| `SET 1, A` | `CB CF` | 8 |  |
| `SET 2, B` | `CB D0` | 8 |  |
| `SET 2, C` | `CB D1` | 8 |  |
| `SET 2, D` | `CB D2` | 8 |  |
| `SET 2, E` | `CB D3` | 8 |  |
| `SET 2, H` | `CB D4` | 8 |  |
| `SET 2, L` | `CB D5` | 8 |  |
| `SET 2, A` | `CB D7` | 8 |  |
| `SET 3, B` | `CB D8` | 8 |  |
| `SET 3, C` | `CB D9` | 8 |  |
| `SET 3, D` | `CB DA` | 8 |  |
| `SET 3, E` | `CB DB` | 8 |  |
| `SET 3, H` | `CB DC` | 8 |  |
| `SET 3, L` | `CB DD` | 8 |  |
| `SET 3, A` | `CB DF` | 8 |  |
| `SET 4, B` | `CB E0` | 8 |  |
| `SET 4, C` | `CB E1` | 8 |  |
| `SET 4, D` | `CB E2` | 8 |  |
| `SET 4, E` | `CB E3` | 8 |  |
| `SET 4, H` | `CB E4` | 8 |  |
| `SET 4, L` | `CB E5` | 8 |  |
| `SET 4, A` | `CB E7` | 8 |  |
| `SET 5, B` | `CB E8` | 8 |  |
| `SET 5, C` | `CB E9` | 8 |  |
| `SET 5, D` | `CB EA` | 8 |  |
| `SET 5, E` | `CB EB` | 8 |  |
| `SET 5, H` | `CB EC` | 8 |  |
| `SET 5, L` | `CB ED` | 8 |  |
| `SET 5, A` | `CB EF` | 8 |  |
| `SET 6, B` | `CB F0` | 8 |  |
| `SET 6, C` | `CB F1` | 8 |  |
| `SET 6, D` | `CB F2` | 8 |  |
| `SET 6, E` | `CB F3` | 8 |  |
| `SET 6, H` | `CB F4` | 8 |  |
| `SET 6, L` | `CB F5` | 8 |  |
| `SET 6, A` | `CB F7` | 8 |  |
| `SET 7, B` | `CB F8` | 8 |  |
| `SET 7, C` | `CB F9` | 8 |  |
| `SET 7, D` | `CB FA` | 8 |  |
| `SET 7, E` | `CB FB` | 8 |  |
| `SET 7, H` | `CB FC` | 8 |  |
| `SET 7, L` | `CB FD` | 8 |  |
| `SET 7, A` | `CB FF` | 8 |  |

## References

- [Zilog Z80 CPU User Manual](https://www.zilog.com/docs/z80/um0080.pdf)
- [Z80 instruction set, MSX Assembly Page](https://map.grauw.nl/resources/z80instr.php)
