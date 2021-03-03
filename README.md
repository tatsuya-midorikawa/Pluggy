# Pluggy

**`Pluggy (ﾌﾟﾗｯｷﾞｰ)`** はプラグイン機能を簡単に実装するための基本機能を提供するライブラリです。

# 使い方

## 構成

最小で以下のような3要素からなる構成となります。

- Outlet : Pluginを利用する本体
- Plugin : 拡張機能を提供するdll
- Interface : OutletとPluginを繋ぐInterfaceを定義したdll

![module](https://raw.githubusercontent.com/tatsuya-midorikawa/Pluggy/main/assets/module.png)

## Interface

まず、OutletとPluginで共有するインターフェースを定義します。  
これはOutletとPlugin間で交わされる契約となります。

```cs
// [IPlugin.dll]
// interface module
namespace Contract
{
  public interface IPlugin
  {
    void Print();
  }
}
```

## Plugin

Plugin側は機能を提供するクラス・構造体に `PluginAttribute` を付与します。  
またOutlet側と共有するための `Interface module` を参照する必要があります。

```cs
// [SamplePlugin.dll]
// plugin module
using Pluggy;

[Plugin]
public class SamplePlugin : Contract.IPlugin
{
  public void Print()
  {
    System.Console.WriteLine(GetType().FullName);
  }
}
```

## Outlet

特定のフォルダにあるdllからPlugin機能を持つクラス・構造体を呼び出して利用します。  
OutletからInterfaceへの参照が必要ですが、Pluginへの参照追加は当然不要です。

```cs
// [Outlet.exe]
// Outlet module
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Pluggy;

namespace OutletProject
{
  class Program
  {
    static async Task Main(string[] args)
    {
      // plugin dllが存在するパスを指定
      var dir = "./";

      // plugin dllに接続
      // Contract.IPluginを実装しているクラス・構造体の中でPluginAttributeが付与されているもののみ対象
      var outlet = await Outlet<Contract.IPlugin>.ConnectAsync("./");

      // plugin dllからplugin機能を取得
      var plugins = await outlet.GetPluginsAsync();

      foreach (var plugin　in plugins)
      {
        // pluginを活性化して、機能を呼び出す
        var p = await plugin.ActivateAsync();
        p.Print();
      }
    }
  }
}
```

pluginに引数を必要とするコンストラクタが存在する場合、**ActivateAsync()** の引数にパラメータを渡すことも可能です。

```cs
foreach (var plugin　in plugins)
{
  plugin.Activate("parameter", 100).Print();
}
```