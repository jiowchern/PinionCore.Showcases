# Repository Guidelines

## 项目结构与模块组织
- 主要代码位于 `PinionCore.Remote/`，解决方案为 `PinionCore.Remote/PinionCore.sln`。
- 模块：`PinionCore.Remote.*`、`PinionCore.Network`、`PinionCore.Serialization`、`PinionCore.Utility`。
- 测试：`*.Test` 与 `PinionCore.Integration.Tests` 项目（NUnit）。
- 示例：`PinionCore.Samples.*`（如 `HelloWorld.Client`、`Helloworld.Server`）。
- 其它：`games/` 展示用；子模块见 `.gitmodules`；本地配置在 `.editorconfig`（位于 `PinionCore.Remote/`）。

## 构建、测试与本地开发
- 还原并构建（Release）：`dotnet build PinionCore.Remote/PinionCore.sln -c Release`
- 运行全部测试：`dotnet test PinionCore.Remote/PinionCore.sln -c Release`
- 覆盖率（coverlet.msbuild 集成）：`dotnet test PinionCore.Remote/PinionCore.sln /p:CollectCoverage=true`
- 运行示例：
  - 服务器：`dotnet run --project PinionCore.Remote/PinionCore.Samples.Helloworld.Server`
  - 客户端：`dotnet run --project PinionCore.Remote/PinionCore.Samples.HelloWorld.Client`

## 代码风格与命名规范
- 遵循 .NET/C# 常规约定；`PinionCore.Remote/.editorconfig` 生效。
- 缩进 4 空格，UTF-8；明显类型用 `var`；公共 API 添加注释。
- 命名：公共类型/成员用 PascalCase，本地变量/字段用 camelCase；异步方法以 `Async` 结尾。
- 提交前可运行：`dotnet format`（若环境可用）。

## 测试规范
- 框架：NUnit；Mock：NSubstitute；集成测试放在 `PinionCore.Integration.Tests`。
- 文件命名以 `*Tests.cs` 结尾；方法名清晰表达行为（如 `AgentDisconnectTest`）。
- 选择性运行（如使用分类时）：`dotnet test ... --filter TestCategory=Unit`。
- 覆盖新增/变更的公共 API 与关键网络流程，优先快测（单元）再做端到端。

## 提交与 Pull Request
- 提交信息使用祈使句，简明主题（50–72 字符），可引用 Issue：`Fix: handle peer disconnect (#123)`。
- PR 需包含：变更摘要、关联 Issue、重现与验证步骤、测试覆盖说明；涉及协议/网络请附运行日志或截图。
- 合并前确保：构建通过、`dotnet test` 全绿；涉及样例的改动需验证示例可运行。

## 安全与配置提示
- 禁止提交密钥；本地用环境变量注入配置与端口。
- 测试尽量使用回环地址与临时端口（示例中有获取可用端口工具）。
