-- 窗口的布局样式
-- 样式优先级->用户指定显示样式->用户的样式根据分辨率适应->系统样式->程序内置设计布局
CREATE TABLE sys_t_layout_style
(
    Id           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,--GUID
    FormTypeId   VARCHAR(6)       NOT NULL,--窗口ID sys_t_oper_type
    ScreenWidth  INT              NOT NULL,--屏幕宽度 可用于自动筛选适用的样式
    ScreenHeight INT              NOT NULL,--屏幕高度 可用于自动筛选适用的样式
    IsSystem     BIT              NOT NULL DEFAULT 0,--是否为系统样式
    Enable       BIT              NOT NULL DEFAULT 1,--是否启用
    LayoutData   VARCHAR(MAX)     NOT NULL,--样式xml
    OperId       VARCHAR(50)      NOT NULL,--设计人
    CreateTime   DATETIME         NOT NULL DEFAULT GETDATE(),--创建日期
    UpdateTime   DATETIME         NOT NULL DEFAULT GETDATE(),--修改日期 本地缓存样式以UpdateTime更新缓存
    AppVersion   VARCHAR(255)     NOT NULL--程序版本号，用于现有样式和程序又加了控件的对比,可以提示窗口样式已过期 生成规则:窗口的所有控件Name、数量等参与运算MD5等
)
--样式操作员关联
CREATE TABLE sys_t_layout_oper
(
    OperId         VARCHAR(32)      NOT NULL,--操作员
    LayoutId       UNIQUEIDENTIFIER NOT NULL,--样式ID
    IsEnforceUsing BIT              NOT NULL DEFAULT 0,--是否强制使用
    CreateTime     DATETIME         NOT NULL DEFAULT GETDATE(),--创建日期
    UpdateTime     DATETIME         NOT NULL DEFAULT GETDATE(),--修改日期
    PRIMARY KEY (OperId, LayoutId)
)

--窗口样式的业务扩展(预留)
--待讨论:可以实现业务上的样式自动切换 例如：单001编辑时用的A样式，预览的时候可以自动切到A样式展示
CREATE TABLE sys_t_layout_business
(
    FormTypeId       VARCHAR(6)       NOT NULL,--窗口ID sys_t_oper_type,
    OperId           VARCHAR(32)      NOT NULL,--操作员
    LayoutId         UNIQUEIDENTIFIER NOT NULL,--样式ID
    SheetNo          VARCHAR(50)      NULL,
    SupCusFlag       VARCHAR(1)       NULL,
    SupCusNo         VARCHAR(50)      NULL,
    UndefinedColumn  VARCHAR(255)     NULL,--预留列
    UndefinedColumn2 VARCHAR(255)     NULL,
    UndefinedColumn3 VARCHAR(255)     NULL,
    UndefinedColumn4 VARCHAR(255)     NULL,
    UndefinedColumn5 VARCHAR(255)     NULL,
    CreateTime       DATETIME         NOT NULL DEFAULT GETDATE()
)