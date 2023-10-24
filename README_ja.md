# MemoizationForUnity
## 概要
本ライブラリ「MemoizationForUnity」は強力なメモ化機能を提供します。  
メモ化とは関数のインプットをキーとしてアウトプットをキャッシュする機能です。  
それによって、2回目以降の重い計算処理をスキップし即座にアウトプットを返す事が可能になります。  
引数をキーにして戻り値をキャッシュするテクニックは古くからありますが  
その中でもメモ化は関数型言語系でメジャーなテクニックです。  
「MemoizationForUnity」はSourceGeneratorを用いてメモ化のキャッシュ機能を自動生成します。  

## 動作確認環境
|  環境  |  バージョン  |
| ---- | ---- |
| Unity | 2021.3.15f1, 2022.3.2f1 |
| .Net | 4.x, Standard 2.1 |

## 性能
### エディタ上の計測コード
[テストコード](packages/Tests/Runtime/MemoizationPerformanceTest.cs)  

#### 結果

|  実行処理  |  Raw  |  Memoized  |
| ---- | ---- | ---- |
| CalcPrime | 76.053645 ms | 0.55148 ms |
| GetMethodInfo | 11.74201 ms | 2.711665 ms |
| GetTypes | 190.787115 ms | 0.00019 ms |

CalcPrimeは1つの引数、GetMethodInfoは3つの引数を使っています。  
すべての項目において性能が向上しています。  
引数が少ないほど取得が早く、Rawの処理負荷が高いほど効果が高いことがわかります。  
引数を持たず負荷の高いGetTypesは1,000,000倍程度の処理速度向上が見られます。  

### Releaseビルド後の計測コード
```.cs
private readonly ref struct Measure
{
    private readonly string _label;
    private readonly StringBuilder _builder;
    private readonly float _time;

    public Measure(string label, StringBuilder builder)
    {
        _label = label;
        _builder = builder;
        _time = (Time.realtimeSinceStartup * 1000);
    }

    public void Dispose()
    {
        _builder.AppendLine($"{_label}: {(Time.realtimeSinceStartup * 1000) - _time} ms");
    }
}

:

var log = new StringBuilder();
{
    using (new Measure("CalcPrime_Raw", log))
    {
        for (int i = 0; i < 10000; ++i)
        {
            FindLargestPrimeRaw(1000);
        }
    }

    using (new Measure("CalcPrime_Memoized", log))
    {
        for (int i = 0; i < 10000; ++i)
        {
            FindLargestPrime(1000);
        }
    }
}

 :
```

#### 結果

|  実行処理  |  Raw(Mono)  |  Memoized(Mono)  |  Raw(IL2CPP)  |  Memoized(IL2CPP)  |
| ---- | ---- | ---- | ---- | ---- |
| CalcPrime | 22.59387 ms | 0.1289196 ms | 16.56445 ms | 0.1796875 ms |
| GetMethod | 9.625164 ms | 0.793745 ms | 21.50146 ms | 1.361328 ms |
| GetTypes | 29.42586 ms | 0.000120163 ms | 3.352539 ms | 0 ms(測定不能) |

いずれの項目もエディタよりも性能が向上しています。  
この検証で、キャッシュ参照はIL2CPPよりもMonoのほうが性能が高くなることが明らかになりました。  
GetTypesはStatic Type Cachingによる最適化対象のため最速の性能を持っています。  

## インストール方法
### MemoizationForUnityのインストール
1. [Window > Package Manager]を開く。
2. [+ > Add package from git url...]をクリックする。
3. `https://github.com/Katsuya100/MemoizationForUnity.git?path=packages`と入力し[Add]をクリックする。

#### うまくいかない場合
上記方法は、gitがインストールされていない環境ではうまく動作しない場合があります。
[Releases](https://github.com/Katsuya100/MemoizationForUnity/releases)から該当のバージョンの`com.katuusagi.memoizationforunity.tgz`をダウンロードし
[Package Manager > + > Add package from tarball...]を使ってインストールしてください。

#### それでもうまくいかない場合
[Releases](https://github.com/Katsuya100/MemoizationForUnity/releases)から該当のバージョンの`Katuusagi.MemoizationForUnity.unitypackage`をダウンロードし
[Assets > Import Package > Custom Package]からプロジェクトにインポートしてください。

## 使い方
### 通常の使用法
partial型内のメソッドに`Memoization`属性をつけることでMemoization関数を生成します。  
```.cs
private partial class Class
{
    [Memoization]
    public static int Method(int arg0)
    {
        :
    }
}
```
すると下記のような実装が生成されます。  
```.cs
private static System.Collections.Generic.Dictionary<int, int> __MemoizationCacheValue_980d929936fa49b59a9d533a1c442eca__ = new System.Collections.Generic.Dictionary<int, int>();
public static int MethodWithMemoization(int arg0)
{
    var __key__ = arg0;
    if (__MemoizationCacheValue_980d929936fa49b59a9d533a1c442eca__.TryGetValue(__key__, out var __cache__))
    {
        return __cache__;
    }
    var __result__ = Method(arg0);
    __MemoizationCacheValue_980d929936fa49b59a9d533a1c442eca__.Add(__key__, __result__);
    return __result__;
}
```
`MethodWithMemoization`は戻り値をキャッシュしているため、高速に戻り値を返却します。  
  
あるいは、以下のように`Raw`のサフィックスをつけることで`WithMemoization`のサフィックスを省略できます。  
```.cs
private partial class Class
{
    [Memoization]
    public static int MethodRaw(int arg0)
    {
        :
    }
}
```
`Raw`サフィックスを使うことで以下の実装が生成されます。  
```.cs
private static System.Collections.Generic.Dictionary<int, int> __MemoizationCacheValue_e99db10c9a424507be15cbcd47f085c3__ = new System.Collections.Generic.Dictionary<int, int>();
public static int Method(int arg0)
{
    var __key__ = arg0;
    if (__MemoizationCacheValue_e99db10c9a424507be15cbcd47f085c3__.TryGetValue(__key__, out var __cache__))
    {
        return __cache__;
    }
    var __result__ = MethodRaw(arg0);
    __MemoizationCacheValue_e99db10c9a424507be15cbcd47f085c3__.Add(__key__, __result__);
    return __result__;
}
```
この場合の`Method`も戻り値をキャッシュします。

### キャッシュクリア
キャッシュクリアを行いたい場合は`Memoization`属性の`IsClearable`パラメーターを設定します。  
```.cs
[Memoization(IsClearable = true)]
public static int MethodRaw(int arg0)
{
    :
}
```
すると対象に応じたClear関数が生成されます。  
Instance関数であれば`ClearInstanceMemoizationCache`が生成されます。  
Static関数であれば`ClearStaticMemoizationCache`が生成されます。  

### 生成された関数名のカスタム
`MethodName`パラメーターを設定することで、自由な関数名を使用できます。  
以下の例では`MethodFast`という関数名を設定しています。  
```.cs
[Memoization(MethodName = nameof(MethodFast))]
public static int MethodInternal(int arg0)
{
    :
}
```
`MethodName`パラメーターを参照し以下のような実装が生成されます。  
```.cs
private static System.Collections.Generic.Dictionary<int, int> __MemoizationCacheValue_c119bf728a8546a282d1fa37ffa9d7e6__ = new System.Collections.Generic.Dictionary<int, int>();
public static int MethodFast(int arg0)
{
    var __key__ = arg0;
    if (__MemoizationCacheValue_c119bf728a8546a282d1fa37ffa9d7e6__.TryGetValue(__key__, out var __cache__))
    {
        return __cache__;
    }
    var __result__ = MethodInternal(arg0);
    __MemoizationCacheValue_c119bf728a8546a282d1fa37ffa9d7e6__.Add(__key__, __result__);
    return __result__;
}
```

### スレッドセーフ対応
Memoizationのキャッシュは基本的にはスレッドセーフではありません。  
複数のスレッドから呼び出す関数には`Memoization`属性の`ThreadSafeType`パラメーターを設定します。  
```.cs
[Memoization(ThreadSafeType = ThreadSafeType.Concurrent)]
public static int MethodRaw(int arg0)
{
    :
}
```
`ThreadSafeType`パラメーターを設定することでキャッシュ領域のハッシュテーブルがスレッドセーフ化されます。  
設定する値によって処理が異なります。  

| ThreadSafeType | 説明 |  
|  ---- | ---- |  
| Concurrent | ThreadSafeなコレクションを用いてキャッシュする。 |  
| ThreadStatic | キャッシュにThreadStatic属性が付与される。<br/>Thread毎に異なるキャッシュが使用される。 |

### 修飾子のカスタム
Memoizationのデフォルト挙動では元のメソッドの修飾子を引き継ぎます。  
しかし、計算処理は内部メソッドにして生成されたメソッドのみを公開したい場合があるかと思います。  
そういった場合は`Modifier`パラメーターを使用します。  
```.cs
[Memoization(Modifier = "public static")]
private static int MethodRaw(int arg0)
{
    :
}
```
すると、下記の実装が生成されます。  
修飾子がpublicに変わっているのが確認できます。  
```.cs
private static System.Collections.Generic.Dictionary<int, int> __MemoizationCacheValue_9c36aae2f3ed4842919001270b87e3c0__ = new System.Collections.Generic.Dictionary<int, int>();
public static int Method(int arg0)
{
    var __key__ = arg0;
    if (__MemoizationCacheValue_9c36aae2f3ed4842919001270b87e3c0__.TryGetValue(__key__, out var __cache__))
    {
        return __cache__;
    }
    var __result__ = MethodRaw(arg0);
    __MemoizationCacheValue_9c36aae2f3ed4842919001270b87e3c0__.Add(__key__, __result__);
    return __result__;
}
```
### 配列要素を考慮するには
通常配列が引数で渡される場合、参照をキーの一部としてしまうため都合の悪い場合がある。
配列の要素をキーとして扱うには下記のように実装すると良い。
```.cs
[Memoization(CompareArrayElement = true)]
private static int MethodRaw(int arg0)
{
    :
}
```

### 上級者向けの機能
#### キャッシュ比較処理のカスタム
Memoizationはキャッシュをハッシュテーブルで参照します。  
内部的にも可能な限り最適化していますが、以下のように`CacheComparer`パラメーターをカスタムすることで更なる高速化が可能です。  
```.cs
public class CustomComparer : IEqualityComparer<int>
{
  :
}

[Memoization(CacheComparer = "new CustomComparer()")]
private static int MethodRaw(int arg0)
{
    :
}
```
すると実装されるDictionaryに`CustomComparer`が割り当てられます。  
```
private static System.Collections.Generic.Dictionary<int, int> __MemoizationCacheValue_eddb945ad3a8410db30f0a8eebf6af87__ = new System.Collections.Generic.Dictionary<int, int>(new CustomComparer());
```

#### キャッシュ生成時のコールバックを作成
キャッシュが作成されたことを検知したい場合は`OnCachedMethod`パラメーターに関数名を設定します。
```.cs
[Memoization(OnCachedMethod = nameof(OnCached))]
private static int MethodRaw(int arg0)
{
    :
}
```
partial関数が定義されますのでpartial定義に従って実装をカスタムしましょう。
```.cs
private partial static void OnCached(int key, int result)
{
    :
}
```

#### キャッシュへの割り込み
キャッシュの事前作成やMemoizationメソッド間でキャッシュの共有をしたい場合があります。  
そういった場合は`InterruptCacheMethod`パラメーターに関数名を設定します。  
```.cs
[Memoization(InterruptCacheMethod = nameof(InterruptCache))]
private static int MethodRaw(int arg0)
{
    :
}
```
するとキャッシュへの値の追加や変更を行う関数が生成されます。  
ただし、この操作はキャッシュ内を自由に操作できてしまうため想定外の動作を引き起こす危険があります。  

## 禁止されている設定
### partialでない型への適用
SourceGeneratorでコードを生成する関係上、partialでない型にはMemoizationを適用することは出来ません。

### structのインスタンスメソッドへの適用
structのインスタンスメンバをpartialすると、回避不能の警告(CS0282)が発生します。  
そのためstructのインスタンスメソッドへのMemoizationの適用は禁止しています。  

### 戻り値やout,refを持たないメソッドへの適用
アウトプットのないメソッドにはMemoizationを設定できません。  

## 高速な理由
このライブラリではSourceGeneratorを用いてコードを解析しケースバイケースで最適なキャッシュ方法を選定しています。  
例えば、引数を持つメソッドの場合一度Tuple化し、Tuple専用に最適化したEqualityComparerを用います。  
引数がないメソッドならばキャッシュしたかどうかをフラグで確認するのみです。  
そのため高速に値を取ってくることが可能です。  
さらにキャッシュクリアする必要がなければStatic Type Cachingによる最適化が行われます。  
この場合、ゼロコストでキャッシュを取得できるようになります。  
GenericなStatic関数の場合もStatic Type Cachingによって参照先のテーブルが切り替わります。  
型引数しかない場合はStatic Type Cachingが直接値を保持するため、ゼロコストでキャッシュを取得できます。  
これを利用すれば面倒くさいStatic Type Cachingの定義を書く必要がなくなります。  
Instance関数の場合はStatic Type Cachingは使えませんが型引数を整数のIDに変換しているためTypeをキーにするより高速にキャッシュを検索できます。  
  
これらのテクニックにより高速にキャッシュを取得できるようになっています。  

### 注意点
Memoizationの理論上最も遅くなるのは、ThreadSafeかつ長大な可変長引数を持つInstance関数です。  
高速化に寄与できる場面もありますが、これらの条件が揃っている場合は注意して扱ってください。  

また、浮動小数を引数に取る場合も注意が必要です。  
リニアに変化する値を引数に取る場合キャッシュサイズが増加しやすい傾向にあります。  
また、現在時刻などを引数に取る場合も同様です。  
「再度同じ値を使って計算を行う」といったケースに絞って適用してください。  
