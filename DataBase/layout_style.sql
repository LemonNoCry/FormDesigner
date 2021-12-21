-- 窗口的布局样式
CREATE TABLE sys_t_layout_style
(
    Id         VARCHAR(50)  NOT NULL PRIMARY KEY,--GUID
    FormTypeId VARCHAR(6)   NOT NULL,--窗口ID sys_t_oper_type
    
    LayoutData VARCHAR(MAX) NULL,--样式xml
    CreateTime DATETIME     NOT NULL DEFAULT GETDATE(),--创建日期
    UpdateTime DATETIME     NOT NULL DEFAULT GETDATE(),--修改日期
)

CREATE TABLE sys_t_layout_oper
(
    OperId   VARCHAR(32) NOT NULL,--操作员
    LayoutId VARCHAR(50) NOT NULL,--样式ID
)

SELECT * FROM sa_t_operator_i;