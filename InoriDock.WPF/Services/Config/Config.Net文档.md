# Config.Net

[![NuGet](https://img.shields.io/nuget/v/Config.Net.svg)](https://www.nuget.org/packages/Config.Net) [![Open Collective backers and sponsors](https://img.shields.io/opencollective/all/config?label=opencollective%20sponsors)](https://opencollective.com/config) [![GitHub Sponsors](https://img.shields.io/github/sponsors/aloneguid?label=github%20sponsors)](https://github.com/sponsors/aloneguid) [![Nuget](https://img.shields.io/nuget/dt/Config.Net)](https://www.nuget.org/packages/Config.Net)

Config.Net 是一个全面、易于使用且功能强大的 .NET 配置库，它通过单元测试全面覆盖，并已在数千台服务器和应用程序中经过实战测试。

该库解决了配置分散在不同位置、需要在不同提供程序之间转换类型、在解决方案中硬编码配置键以及依赖于特定配置源实现等问题。它通过提供一个抽象的配置接口，并为常见的配置源（如 `app.config`、环境变量等）提供最常用的实现来解决这些问题。

## 快速入门

通常，开发人员会从不同的源（如 app.config、本地 json 文件等）硬编码读取配置值。例如，考虑以下代码示例：

```csharp
var clientId = ConfigurationManager.AppSettings["AuthClientId"];
var clientSecret = ConfigurationManager.AppSettings["AuthClientSecret"];
```

你可能会猜测这段代码试图按名称从本地 app.config 文件中读取配置设置，这可能是真的，但这种方法存在诸多问题：

- 配置项通过硬编码的字符串名称引用，容易出现拼写错误，从而导致运行时崩溃。
- 除了执行全文搜索（前提是字符串没有拼写错误）之外，没有简单的方法可以找出特定设置在代码中的使用位置。
- 如果你决定将配置存储在不同的位置，则必须重写代码。

欢迎使用 Config.Net，它解决了上述大部分问题。让我们使用 Config.Net 的方法重写这段代码。首先，我们需要定义一个配置容器，描述你的应用程序或库中使用的设置：

### 声明设置接口

```csharp
using Config.Net;

public interface IMySettings
{
    string AuthClientId { get; }

    string AuthClientSecret { get; }
}
```

这些接口成员描述了你在代码中使用的值，看起来与代码中的其他内容完全一样。你可以在应用程序中像平常一样传递这个接口。

要实例化这个接口并将其绑定到应用程序设置，请使用 `ConfigurationBuilder<T>` 类：

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseAppConfig()
   .Build();
```

这便是你需要做的全部工作。配置生成器是创建接口实例的入口，在底层，它会创建一个代理类，该类拦截对属性的调用，并从已配置的存储中获取值。

### 支持哪些数据类型？

并非所有类型都可以用作属性，因为 Config.Net 需要知道如何将它们转换为底层存储中的格式以及从底层存储中转换回来。默认情况下，支持基本的 .NET 类型（`bool`、`double`、`int`、`long`、`string`、`TimeSpan`、`DateTime`、`Uri`、`Guid`）。还有两个类型值得一提：

#### `System.Net.NetworkCredential`

这是一个方便的内置 .NET 类，用于保存用户名、密码和域的信息。实际上，这三个字段几乎总是足以保存对远程服务器的连接信息。Config.Net 理解以下格式：`username:password@domain`，所有部分均可选。

#### 字符串数组

使用命令行语法进行编码：

- 值之间用空格分隔，例如 `value1 value2`
- 如果值中需要包含空格，则必须用引号括起来，例如 `"value with space" valuewithoutspace`
- 值内的引号需要用双引号转义（`""`），并且整个值本身应该用引号括起来，例如 `"value with ""quotes""""`

通过实现 `ITypeParser` 接口，可以轻松添加新类型。

### 使用多个源

`ConfigurationBuilder<T>` 用于实例化你的配置接口。你可以使用它来添加多个配置源。要获取源列表，请使用 IntelliSense（输入点-Use）：

![Intellisense00](intellisense00.png)

添加源的顺序很重要——Config.Net 会按照配置的顺序尝试读取源，并返回第一个存在该设置的存储中的值。

### 改变属性行为

可以使用 `Option` 属性来为接口属性添加额外的行为。

#### 别名

如果属性的名称与 C# 属性名称不同，你可以为其设置别名：

```csharp
public interface IMySettings
{
   [Option(Alias = "clientId")]
   string AuthClientId { get; }
}
```

这使得 Config.Net 在读取或写入时会寻找 "clientId"。

#### 默认值

当属性在任何存储中都不存在，或者你根本还没有配置任何存储时，你将收到该属性类型的默认值（`int` 类型为 0，`string` 类型为 `null` 等）。然而，有时返回一个不同的默认值会很有用，而不是在代码中处理这种情况。为此，你可以使用属性上的 `DefaultValue` 属性：

```csharp
public interface IMySettings
{
   [Option(Alias = "clientId", DefaultValue = "n/a")]
   string AuthClientId { get; }
}
```

现在，读取值时将返回 `n/a`，而不是仅仅返回 `null`。`DefaultValue` 属性的类型为 `object`，因此你分配给它的值的类型必须与属性类型匹配。如果不是这样，你将在 `.Build()` 阶段收到 `InvalidCastException`，它会解释问题所在。

然而，你可以将属性值设置为 `string`，无论其类型是什么，只要它可以在运行时使用任何解析器解析为那种类型即可。

##### DefaultValueAttribute

Config.Net 还支持 [DefaultValueAttribute](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.defaultvalueattribute?view=netcore-3.1) 作为指定默认值的替代方法。这允许你的接口不依赖于 Config.Net 库。以下定义具有相同的效果：

```csharp
public interface IMySettings
{
   [Option(DefaultValue = "n/a")]
   string AuthClientId { get; }
}
```

```csharp
public interface IMySettings
{
   [DefaultValue("n/a")]
   string AuthClientId { get; }
}
```

### 写入设置

某些配置存储支持写入值。可以通过检查 `IConfigStore.CanWrite` 属性来确定。你可以通过简单地设置其值来写回值：

```csharp
c.AuthClientId = "new value";
```

Config.Net 会将值写入所有支持写入的已配置存储。如果没有任何存储支持写入，则会忽略该调用。

当然，为了使属性可写，你需要在接口中这样声明它：

```csharp
string AuthClientId { get; set; }
```

## 嵌套接口

接口可以相互嵌套。当你有一组类似的设置，并且不想重新声明它们时，这会很有用，例如，假设我们想在配置存储中存储普通用户和管理员的凭据。

首先，我们可以声明一个通用的接口来存储凭据：

```csharp
public interface ICreds
{
   string Username { get; }

   string Password { get; }
}
```

然后将其包含在我们的配置接口中：

```csharp
public interface IConfig
{
   ICreds Admin { get; }

   ICreds Normal { get; }
}
```

然后，实例化 `IConfig`：

```csharp
_config = new ConfigurationBuilder<IConfig>()
   .Use...
   .Build();
```

现在，你可以使用正常的 C# 语法获取凭据，例如，要获取管理员用户名 `_config.Admin.Username` 等。

所有属性仍然适用于嵌套接口。

在获取属性值时，每个嵌套级别将用点 (`.`) 分隔，例如，管理员用户名是通过键 `Admin.Username` 获取的——在使用平面配置存储时需要注意这一点。

## 集合

Config.Net 支持原始类型和**接口**的集合。

集合必须始终声明为 `IEnumerable<T>`，并且只能有 getter。

目前，集合仅支持只读模式，未来版本可能会支持写入集合。

### 原始类型

假设你想读取一个整数数组，可以这样声明：

```csharp
interface IMyConfig
{
   IEnumerable<int> Numbers { get; }
}
```

### 接口

读取原始类型的数组并不那么有趣，因为你可以通过在字符串中存储某种分隔符（如 `1,2,3,4`）来自己实现解析。Config.Net 允许你读取自己的复杂类型的集合，如下所示：

```csharp
interface ICredentials
{
   string Username { get; }
   string Password { get; }
}

interface IMyConfig
{
   IEnumerable<ICredentials> AllCredentials { get; }
}
```

### 限制

目前，集合仅支持只读模式。所有存储都支持集合，但由于某些底层实现的性质，复杂集合更难表示，它们遵循 [平面语法](#flatline-syntax)。

## 绑定到方法

有时，仅仅有 setter 和 getter 是不够的，或者你需要在运行时才知道名称的配置键。这时，动态配置就派上用场了。

通过动态配置，你可以在配置接口中声明方法，而不仅仅是属性。看看下面这个例子：

```csharp
public interface ICallableConfig
{
   string GetName(string keyName);
}
```

调用该方法时，Config.Net 将使用键 **Name**.*`keyName` 的值* 来读取配置。例如，调用该方法为 `.GetName("my_key")` 时，将返回键为 `Name.my_key` 的值。

键的第一部分来自方法名称本身（会自动移除任何 `Get` 或 `Set` 方法前缀）。如果你想要自定义键名，可以使用 `[Option]` 属性，例如：

```csharp
public interface ICallableConfig
{
   [Option(Alias = "CustomName")]
   string GetName(string keyName);
}
```

这将把键改为 **CustomName**.*`keyName` 的值*。

请注意，如果你声明一个方法为 `Get(string name)`，Config.Net 将从根命名空间读取设置，例如 `Get("myprop")` 将返回键为 `myprop` 的值。本质上，这允许你 *动态地从存储中读取*，然而，你将失去执行类型安全转换的能力。

### 多个参数

你可以声明具有任意数量参数的方法，它们将简单地串联起来以决定使用哪个键名，例如：

```csharp
public interface ICallableConfig
{
   string GetName(string keyName, string subKeyName);
}
```

将使用键 **Name**.*`keyName` 的值*.*`subKeyName` 的值* 来读取配置等。

### 写入值

与声明读取值的方法一样，你也可以声明用于写入值的方法。唯一的区别是，写入值的方法 *必须是 void*。写入方法的最后一个参数被视为值参数，例如：

```csharp
public interface ICallableConfig
{
   void SetName(string keyName, string value);
}
```

## 支持 INotifyPropertyChanged

INotifyPropertyChanged 是 .NET Framework 的一部分，通常用于在你想要监视类属性更改时使用。它也是 **Xamarin**、**WPF**、**UWP** 和 **Windows Forms** 数据绑定系统的重要组成部分。

Config.Net 完全支持 `INPC` 接口，你只需要让接口继承自 `INPC`：

```csharp
public interface IMyConfiguration : INotifyPropertyChanged
{
   string Name { get; set; }
}
```

然后像往常一样构建你的配置，并订阅属性更改事件：

```csharp
IMyConfiguration config = new ConfigurationBuilder<IMyConfiguration>()
   //...
   .Build();

config.PropertyChanged += (sender, e) =>
{
   Assert.Equal("Name", e.PropertyName);
};

config.Name = "test";   //这将触发 PropertyChanged 委托
```

## 平面语法

### 复杂结构

许多提供程序不支持嵌套结构。假设你有以下配置声明：

```csharp
//集合元素
public interface IArrayElement
{
   string Username { get; }

   string Password { get; }
}

//顶层配置
public interface IConfig
{
   IEnumerable<IArrayElement> Creds { get; }
}
```

你想传递两个元素，它们在 json 中表示如下：

```json
"Creds": [
   {
      "Username": "user1",
      "Password": "pass1"
   },
   {
      "Username": "user2",
      "Password":  "pass2"
   }
]
```

然而，你使用的是显然不支持嵌套结构的命令行配置提供程序。Config.Net 提出了所谓的 **平面语法**，以便仍然可以使用类似平面的提供程序并传递嵌套结构。上面的例子将转换为：

```bash
myapp.exe Creds.$l=2 Creds[0].Username=user1 Creds[0].Password=pass1 Creds[1].Username=user2 Creds[1].Password=pass2
```

这看起来很有表现力，但它仍然允许你使用嵌套结构。

实际上，你可能不会使用命令行传递大型嵌套结构，而是会覆盖一些默认参数。

### 简单结构

简单结构可以通过将所有值组合在一行上来表示。例如，以下配置：

```csharp
public interface ISimpleArrays
{
   IEnumerable<int> Numbers { get; }
}
```

可以映射到以下命令行：

```bash
myapp.exe Numbers="1 2 3"
```

在 [命令行存储](#command-line) 中描述的为一个参数提供多个值的语法是相同的。

## 配置源

### AppConfig 存储

配置存储：

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseAppConfig()
   .Build();
```

它没有任何参数。它是只读的，并将读取操作转发给标准的 [ConfigurationManager](https://msdn.microsoft.com/en-us/library/system.configuration.configurationmanager%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396) 类。

- 键直接映射到 `<appSettings>` 元素。
- 如果在 _appSettings_ 中找不到键，则会尝试在 `<connectionStrings>` 中查找。
- 如果仍然找不到，并且会尝试查找名称在第一个点分隔符之前的部分对应的节，并从中读取键。

为了演示这一点，考虑以下示例 *app.config*：

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
   <configSections>
      <section name="MySection" type="System.Configuration.NameValueSectionHandler"/>
   </configSections>
   <appSettings>
      <add key="AppKey" value="TestValue"/>
   </appSettings>
   <connectionStrings>
      <add name="MyConnection" connectionString="testconn"/>
   </connectionStrings>
   <MySection>
      <add key="MyKey" value="MyCustomValue"/>
   </MySection>
</configuration>
```

它可以映射到如下配置接口：

```csharp
   public interface IConfig
   {
      string AppKey { get; }

      string MyConnection { get; }

      [Option(Alias = "MySection.MyKey")]
      string MySectionKey { get; }
   }
```

#### 集合

通过使用 [平面语法](#flatline-syntax) 支持集合。

### 命令行

这并不是一个命令行框架，而是允许你在命令行上显式传递配置值的附加功能。

配置存储：

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseCommandLine()
   .Build();
```

#### 约定

此存储将识别任何具有键值分隔符（`=` 或 `:`）的命令行参数，并且可以选择性地以 `/` 或 `-` 开头（存储会从参数开头移除这些字符）。

如果参数有多个分隔符，则使用第一个。

#### 无命名参数

默认情况下，不带名称（没有分隔符）的参数将被跳过。如果你希望将位置参数映射到选项值，可以在配置中指定一个可选的字典（参见下面的示例）。

#### 示例

##### 可识别的参数

`program.exe arg1=value1 arg2:value2 arg3:value:3 -arg4:value4` `--arg5:value5` `/arg6:value6`

所有这些参数都是有效的，本质上它们将变成以下内容：

- `arg1`:`value1`
- `arg2`:`value2`
- `arg3`:`value:3` - 使用第一个分隔符
- `arg4`:`value4`
- `arg5`:`value5`
- `arg6`:`value6`

##### 位置参数

在许多情况下，命令行参数没有名称，但仍然需要被捕获，考虑这个示例：

`myutil upload file1.txt`

这比强迫用户指定命令行参数要短得多：

`myutil /action=upload /filepath=file1.txt`

你可以这样表达配置以捕获它：

```csharp
public interface IConsoleCommands
{
   [Option(DefaultValue = "download")]
   string Action { get; }

   string FilePath { get; }
}

//...

IConsoleCommands settings =
   new ConfigurationBuilder<IConsoleCommands>()
   .UseCommandLineArgs(
      new KeyValuePair<string, int>(nameof(IConsoleCommands.Action), 1),
      new KeyValuePair<string, int>(nameof(IConsoleCommands.FilePath), 2))
   .Build();
```

请注意，第一个命令行参数从 `1` 开始，而不是 `0`。

#### 集合

命令行存储也通过使用 [平面语法](#flatline-syntax) 支持集合。

### 环境变量

配置存储：

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseEnvironmentVariables()
   .Build();
```

此存储与系统环境变量一起工作，这些变量可以通过在 Windows 的 **cmd.exe** 中输入 `set` 或在 PowerShell 中输入 `Get-ChildItem Env:` 或在基于 Unix 的系统中输入 `env` 来获取。

该存储支持读取和写入环境变量。

> 注意：某些系统（如 Visual Studio Team System Build）在定义变量时会将点 (`.`) 替换为下划线 (`_`)。为了克服这一点，存储将尝试读取变量的两种变体。

#### 集合

通过使用 [平面语法](#flatline-syntax) 支持集合。

### `.env` 文件

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseDotEnvFile()
   .Build();
```

将尝试从当前目录开始加载 [`.env`](https://github.com/bkeepers/dotenv) 文件，并向上遍历目录结构直到找到一个。你可以选择传递文件夹路径来指定搜索的起始位置。

### 内存

配置存储：

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseInMemoryDictionary()
   .Build();
```

该存储支持读取和写入，并将配置存储在应用程序内存中。当应用程序重新启动时，所有设置值都会丢失。你可能希望在调试或测试中使用此存储，除此之外，它没有实际应用。

#### 集合

通过使用 [平面语法](#flatline-syntax) 支持集合。

### INI

#### 配置

##### 映射到文件

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseIniFile(filePath)
   .Build();
```

此变体支持读取和写入。

##### 映射到文件内容

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseIniString(contentsOfAnIniFile)
   .Build();
```

此变体仅支持读取，因为你立即传递了完整的文件内容。

#### 使用

该存储完全支持 INI 文件部分。

在最简单的形式中，INI 文件中的每个键都对应于一个选项的名称。例如，一个定义

```csharp
string MyOption { get; }
```

将对应于 INI 文件中的一行：

```ini
MyOption=my fancy value
```

#### 使用部分

部分对应于选项名称中第一个点 (`.`) 之前的部分，例如

```ini
[SectionOne]
MyOption=my fancy value
```

应该使用定义

```csharp
[Option(Alias = "SectionOne.MyOption")]
string MyOption { get; }
```

##### 写入

写入是直接的，但是请注意，如果选项名称中包含点 (`.`)，将默认创建一个部分。

在写入时，无论是行内注释还是新行注释都会被保留：

```ini
key1=value1 ;this comment is preserved
;this comments is preserved too
```

##### 特殊情况

在使用 INI 文件时，有一些特殊情况需要注意：

* 值可以包含等号 (`=`)，它将被视为值的一部分，因为只有第一个等号被视为键值分隔符。
* 显然，键名不能包含 `=`

###### 关于 INI 注释的说明

INI 文件将分号 (`;`) 视为行内注释分隔符，因此你不能在值中包含它。例如，一行像 `key=value; this is a comment` 在理想的 INI 实现中将被解析为

- 键：`key`
- 值：`value`
- 注释：`comment`

然而，在我的经验中，像密钥、连接字符串等值 *确实* 经常包含分号，为了将它们放入 INI 文件，你需要做一个技巧，比如在值的末尾添加一个分号，这样前面提到的字符串将变成 `key=value; this is a comment;`，以被解析为

- 键：`key`
- 值：`value; this is a commment`
- 注释：*none*

尽管这是绝对有效的，这也是 INI 文件应有的工作方式，但当值中有许多分号时，这通常真的很令人沮丧，因为你要么检查它们是否包含分号并添加一个分号到末尾，要么就习惯于在每个值的末尾添加分号。我不认为这些解决方案中的任何一个是实用的，因此，自 v**4.8.0** 起，config.net 默认不解析行内注释（仍然会处理注释行）。这解决了许多围绕 “为什么我的值没有被 config.net 正确解析” 或 “这个软件有漏洞” 等问题的困惑。

如果你想恢复到旧的行为，你可以使用新的签名构造 INI 解析器：

```csharp
.UseIniFile(string iniFilePath, bool parseInlineComments = false);

// 或者

.UseIniString<TInterface>(string iniString, bool parseInlineComments = false);
```

并将最后一个参数传递为 `true`。

* 如果值中包含分号 (`;`)，它是 INI 文件中的注释分隔符，你应该也在值的末尾添加一个分号，因为解析器只将最后一个 `;` 视为注释分隔符。例如 `key=val;ue` 将被读取为 `val`，然而 `key=val;ue;` 将被读取为 `val;ue`。

#### 集合

通过使用 [平面语法](#flatline-syntax) 支持集合。

### JSON

JSON 支持读写模式，并且使用 `System.Text.Json.Nodes` 命名空间。因此，它在 .NET 6 及更高版本中是免费的，但在早期的 .NET 版本中，它将引用 `System.Text.Json` nuget 包 v6。

> JSON 存储目前 **不支持写入集合**，主要是因为缺乏时间来正确实现它。

#### 配置

##### 映射到文件

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseJsonFile(path)
   .Build();
```

此变体支持读取和写入。`Path` 可以是相对路径或绝对路径。

##### 映射到文件内容

```csharp
IMySettings settings = new ConfigurationBuilder<IMySettings>()
   .UseJsonString(path)
   .Build();
```

此变体仅支持读取，因为在这种情况下没有地方可以写入。

#### 使用

在最简单的形式中，JSON 文件中的每个键都对应于一个选项的名称。例如，一个定义

```csharp
public interface IMySettings
{
   string AuthClientId { get; }
   string AuthClientSecreat { get; }
}
```

将对应于以下 JSON 文件：

``` json
{
   "AuthClientId": "Id",
   "AuthClientSecret": "Secret"
}
```

##### 使用具有非平凡 JSON 路径的设置

在一个更高级的，可能也是更典型的情况下，JSON 设置将以一种非平凡的方式嵌套在配置结构中（即，不在根部且名称不相同）。`Option` 属性，结合 `Alias` 属性，指定了到达设置值所需的 JSON 路径。

```csharp
public interface IMySettings
{
   string AuthClientId { get; }
   string AuthClientSecreat { get; }
   
   [Option(Alias = "WebService.Host")]
   string ExternalWebServiceHost { get; }
}
```

将对应于以下 JSON 文件：

``` json
{
   "AuthClientId":"Id",
   "AuthClientSecret":"Secret",
   
   "WebService": {
       "Host": "http://blahblah.com:3000"
   }
}
```

## 与之配合使用

### Azure Functions

Azure function [配置](https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings?tabs=portal) 可以在门户中设置，也可以在本地开发时在设置文件中进行。这纯粹是一种魔法，因为它们最终都会以环境变量的形式暴露出来。`.UseEnvironmentVariables()` 将允许读取这些值。

## 赞助

这个框架是免费的，可以用于免费、开源和商业应用程序。Config.Net（所有代码、NuGets 和二进制文件）均在 [MIT 许可证 (MIT)](https://github.com/aloneguid/config/blob/master/LICENSE) 下提供。它经过了实战测试，并被许多优秀的人和组织使用。所以，请点击神奇的 ⭐️ 按钮，我将不胜感激！🙏 谢谢！

核心团队成员、Config.Net 贡献者以及生态系统中的贡献者都是在业余时间进行这项开源工作。如果你使用 Config.Net，并且希望我们能投入更多时间来改进它，请捐款。这个项目也能提高你的收入/生产力/可用性。

### 赞助商

你的公司使用 Config.Net 吗？问问你的经理或市场团队，你的公司是否会对我们的项目感兴趣。支持将使维护者能够为每个人投入更多时间进行维护和新功能开发。此外，你的公司标志将展示在这里 - 谁不想要一些额外的曝光呢？

## 特别感谢

没有 [Castle DynamicProxy 项目](https://www.castleproject.org/projects/dynamicproxy/)，这个项目就不可能实现 - 它是一个轻量级的 .NET 代理生成器。