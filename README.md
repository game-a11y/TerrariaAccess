# TerrariaAccess

An accessibility mod for Terraria

开发板地址:  https://github.com/game-a11y/TerrariaAccess

是开发板，只做了主菜单的输出，还有bug，主菜单菜单项如果更新，MOD 无法感知到这些更新。
这是游戏本身的限制，如果不修改游戏源码，只做 mod 则无法实时获取菜单的变化。
菜单项的更新全部在局部变量中进行，一帧结束后，这些变量也就消失了。

目前没有想到除了直接修改源码的解决方案，因此【项目停滞】。

一些后续的想法

- 继续完善背包物品的输出
- 结合能显示光标处物品的 https://github.com/Jadams505/Twaila 模组进一步完善光标的输出
- 考虑修改上游 tML 实现对菜单的输出


## dev

- [Mod Skeleton Contents](https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents)
