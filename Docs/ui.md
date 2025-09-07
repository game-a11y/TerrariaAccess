# UI
Terraria.CombatText
Terraria.PopupText

Terraria.GameContent.UI.GameTipsDisplay


## 主菜单

- 允许实时修改菜单项后，输出修改后的选项
- 支持音量等滑块输出

Interface.AddMenuButtons
Interface.ModLoaderMenus

### 单人模式
选择人物 UICharacterSelect
    Main._characterSelectMenu
选择世界 UIWorldSelect
    Main._worldSelectMenu

### 多人模式

### 成就 UIAchievementsMenu

- 按钮【成就】`UITextPanel<LocalizedText>`
- 按钮【成就分类】`UIToggleImage : UIElement`
    - 分类的提示文字在 this.Draw 中生成
        Language.GetTextValue("Achievements.NoCategory");
- 【成就列表项】`UIAchievementListItem : UIPanel`
    - 成就图标 `_achievementIcon`
    - 是否完成 `achievement.IsCompleted`
    - 分类图标 `achievement.Category`
    - 成就名称 `achievement.FriendlyName`
    - 成就说明 `achievement.Description`
- 按钮【返回】`UITextPanel<LocalizedText>`

### 创意工坊 UIWorkshopHub
TODO: 修复键盘导航

- 【标题：创意工坊按钮】` UITextPanel<LocalizedText>`
- 管理模组 UIElement MakeFancyButton
- 开发模组
- 下载模组
- 魔族包
- 导入世界
- 使用资源包
    UIResourcePackSelectionMenu
- 【返回】` UITextPanel<LocalizedText>`
- 【日志】` UITextPanel<LocalizedText>`
    OpenReportsMenu()

### 设置
UIManageControls
Main.ManageControlsMenu

### 制作人员
UI.Credits

### 退出

## 游戏中

### 设置 Settings
DrawSettingButton()
