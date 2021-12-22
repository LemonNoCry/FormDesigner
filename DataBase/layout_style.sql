-- 窗口的布局样式
CREATE TABLE sys_t_layout_style
(
    Id           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,--GUID
    FormTypeId   VARCHAR(6)       NOT NULL,--窗口ID sys_t_oper_type
    ScreenWidth  INT              NOT NULL,--屏幕宽度 可用于自动筛选适用的样式
    ScreenHeight INT              NOT NULL,--屏幕高度 可用于自动筛选适用的样式
    IsSystem     BIT              NOT NULL DEFAULT 0,--是否为系统样式
    Enable       BIT              NOT NULL DEFAULT 1,--是否启用
    LayoutData   VARCHAR(MAX)     NULL,--样式xml
    CreateTime   DATETIME         NOT NULL DEFAULT GETDATE(),--创建日期
    UpdateTime   DATETIME         NOT NULL DEFAULT GETDATE(),--修改日期
    Version      VARCHAR(255)     NOT NULL--版本号 生成规则:窗口的所有控件Name、数量等参与运算
)

CREATE TABLE sys_t_layout_oper
(
    OperId         VARCHAR(32)      NOT NULL,--操作员
    LayoutId       UNIQUEIDENTIFIER NOT NULL,--样式ID
    IsEnforceUsing BIT              NOT NULL DEFAULT 0,--是否强制使用
    PRIMARY KEY (OperId, LayoutId)
)

SELECT * FROM sa_t_operator_i;