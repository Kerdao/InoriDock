# Config.Net 配置库文档

[![NuGet版本](https://img.shields.io/nuget/v/Config.Net.svg)](https://www.nuget.org/packages/Config.Net) ![赞助商](https://img.shields.io/opencollective/all/config?label=开源赞助) ![GitHub赞助](https://img.shields.io/github/sponsors/aloneguid?label=GitHub赞助者) [![下载量](https://img.shields.io/nuget/dt/Config.Net)](https://www.nuget.org/packages/Config.Net)

一个全面、易用且功能强大的.NET配置库，经过完整单元测试，已在数千台服务器和应用程序中实际验证。

## 核心优势

![抽象架构](abstract.webp)

本库解决了配置分散在不同位置、需要在不同提供程序间转换类型、在解决方案中硬编码配置键以及依赖特定配置源实现等问题。它通过暴露抽象配置接口并提供最常见配置源(如app.config、环境变量等)的实现来实现这一目标。

## 快速入门

### 1. 传统配置方式的问题

传统方式直接从不同源读取配置值存在诸多问题：
- 通过硬编码字符串名称引用设置，容易拼写错误导致运行时崩溃
- 难以查找特定设置在代码中的使用位置
- 需要重写代码才能更改配置存储位置

### 2. 使用Config.Net方式

#### 定义配置接口

```csharp
public interface IMySettings
{
    string AuthClientId { get; }
    string AuthClientSecret { get; }
}
```

#### 构建配置实例

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseAppConfig()
   .Build();
```

## 支持的数据类型

### 基础类型
- `bool`, `double`, `int`, `long`, `string`
- `TimeSpan`, `DateTime`, `Uri`, `Guid`

### 特殊类型
- `System.Net.NetworkCredential` - 格式: `username:password@domain`
- 字符串数组 - 使用命令行语法格式

## 多配置源支持

配置源添加顺序很重要，Config.Net会按顺序从第一个找到值的存储中返回值。

```csharp
.UseAppConfig()         // app.config/web.config
.UseEnvironmentVariables() // 环境变量
.UseCommandLineArgs()    // 命令行参数
.UseIniFile()           // INI文件
.UseJsonFile()          // JSON文件
```

## 属性行为定制

### 别名

```csharp
[Option(Alias = "clientId")]
string AuthClientId { get; }
```

### 默认值

```csharp
[Option(DefaultValue = "n/a")]
string AuthClientId { get; }
```

## 嵌套接口

支持接口嵌套来表示复杂配置结构：

```csharp
public interface ICreds
{
   string Username { get; }
   string Password { get; }
}

public interface IConfig
{
   ICreds Admin { get; }
   ICreds Normal { get; }
}
```

## 集合支持

### 基本类型集合

```csharp
IEnumerable<int> Numbers { get; }
```

### 接口集合

```csharp
IEnumerable<ICredentials> AllCredentials { get; }
```

## 方法绑定

支持动态配置方法：

```csharp
public interface ICallableConfig
{
   string GetName(string keyName);
   void SetName(string keyName, string value);
}
```

## 变更通知

支持`INotifyPropertyChanged`接口：

```csharp
public interface IMyConfiguration : INotifyPropertyChanged
{
   string Name { get; set; }
}
```

## 扁平化语法

### 复杂结构表示

对于不支持嵌套结构的提供程序，使用特殊语法：

```bash
myapp.exe Creds.$l=2 Creds[0].Username=user1 Creds[0].Password=pass1
```

## 配置存储源

### AppConfig存储

```csharp
.UseAppConfig()
```

### 命令行存储

```csharp
.UseCommandLineArgs()
```

### 环境变量

```csharp
.UseEnvironmentVariables()
```

### .env文件

```csharp
.UseDotEnvFile()
```

### 内存存储

```csharp
.UseInMemoryDictionary()
```

### INI文件

```csharp
.UseIniFile(filePath)
```

### JSON文件

```csharp
.UseJsonFile(path)
```

## Azure Functions集成

Azure函数配置可通过环境变量读取：

```csharp
.UseEnvironmentVariables()
```

## 赞助支持

本框架采用MIT许可证，可免费用于开源和商业应用。核心团队利用业余时间维护该项目，欢迎赞助支持持续开发。