[English](README.md) / [日本語](#jp)

# ASControllerSDK (Windows)

**USB** および / または **BLE** 経由で Aroma Shooter デバイスに接続し、制御するための Windows .NET SDK です。

本 SDK は以下に分かれています:

-   **Core**: `ASControllerSDK.dll`（公開 API + プラグインローダー）
-   **Plugins**（任意）:
    -   `ASControllerSDK.USB.dll`
    -   `ASControllerSDK.BLE.dll`

プラグイン DLL が存在しない場合でも SDK 自体は動作しますが、そのトランスポート（USB / BLE）の機能のみ利用できません。

---

<a id="jp"></a>

## 日本語

### 目次

1. 前提条件
2. 配布パッケージ構成
3. インストール
4. サンプル（SmokeTest）
5. 使い方
    -   0. セットアップ / 検出
    -   1. Simple 射出 API
    -   2. Intensity 射出 API
6. トラブルシューティング
7. ライセンス

---

<a id="前提条件"></a>

### 前提条件

-   OS: **Windows 10 / 11**
-   ランタイム: **.NET Framework 4.7.2**
-   USB:
    -   デバイスがシリアル/COM として認識されること（必要に応じて USB-シリアルドライバを導入）
-   BLE:
    -   Bluetooth LE 対応アダプタ、および Windows の BLE サポート

---

<a id="配布パッケージ構成"></a>

### 配布パッケージ構成

推奨する最小配布構成:

```
ASControllerSDK-Windows/
├─ lib/
│  └─ net472/
│     ├─ ASControllerSDK.dll
│     ├─ ASControllerSDK.USB.dll        （任意）
│     └─ ASControllerSDK.BLE.dll        （任意）
├─ samples/
│  └─ SmokeTest/
│     ├─ ASControllerSDK.SmokeTest.csproj
│     └─ Program.cs
└─ README.md
```

**重要**: 実行時にプラグインローダーが DLL を見つけられるよう、プラグイン DLL は
`.exe` と同じフォルダに置く（または出力フォルダにコピーされる）必要があります。

---

<a id="インストール"></a>

### インストール

#### .NET（デスクトップアプリ）

1. Core DLL `ASControllerSDK.dll` を参照に追加
2. `.exe` と同じフォルダへプラグイン DLL を配置:
    - `ASControllerSDK.USB.dll`（USB 利用時）
    - `ASControllerSDK.BLE.dll`（BLE 利用時）

#### Unity（Windows）

-   必要な DLL を Unity プロジェクトへ配置:
    -   `Assets/Plugins/`（環境により `Assets/Plugins/x86_64/` 等）
-   Unity の .NET 設定は使用バージョンに合わせて **.NET 4.x** を利用してください。

---

<a id="サンプルsmoketest"></a>

### サンプル（SmokeTest）

最小のコンソールサンプルを `samples/SmokeTest/` に用意しています。

実行方法:

-   Visual Studio で `.csproj` を開いて実行、または
-   MSBuild でコマンドラインビルド

サンプルは以下を行います:

1. `Setup()` を呼び出して利用可能なプラグインを初期化
2. 検出したデバイス一覧を表示
3. Simple の射出 → 停止
4. Intensity の射出 → 停止

---

<a id="使い方"></a>

## 使い方

<a id="0-セットアップ--検出"></a>

### 0. セットアップ / 検出

```csharp
using ASControllerSDK;

var controller = AromaShooterController.SharedInstance;
await controller.Setup();

var devices = controller.GetConnectedDevices();
```

各デバイス情報:

-   `Transport`: USB または BLE
-   `Identifier`: 正規化された ID（通常はシリアル）
-   `DisplayName`: OS 側の生の表示名

---

<a id="1-simple-射出-api"></a>

### 1. Simple 射出 API

全デバイスへ射出:

```csharp
controller.ShootSimple(3000, new int[] { 1, 2, 5 }, internalBooster: true);
```

特定デバイスへ射出:

```csharp
controller.ShootSimple(3000, new int[] { 1, 2, 5 }, true, "ASN3A01192");
```

全停止:

```csharp
controller.StopSimple();
```

特定デバイス停止:

```csharp
controller.StopSimple("ASN3A01192");
```

---

<a id="2-intensity-射出-api"></a>

### 2. Intensity 射出 API

`AromaPort`:

```csharp
public sealed class AromaPort
{
    public int number;        // 1..6
    public int concentration; // 0..100
}
```

強度制御ありで全デバイスへ射出:

```csharp
var ports = new List<AromaPort>
{
    new AromaPort { number = 1, concentration = 50 },
    new AromaPort { number = 2, concentration = 80 },
};

controller.ShootWithIntensity(
    3000,
    ports,
    internalBoosterIntensity: 100,
    externalBoosterIntensity: 0
);
```

強度制御の停止（ポート指定）:

```csharp
controller.StopAllWithIntensity(
    ports: new[] { 1, 2 },
    stopInternalBooster: true,
    stopExternalBooster: false
);
```

---

<a id="トラブルシューティング"></a>

## トラブルシューティング

-   デバイスが見つからない:
    -   先に `await controller.Setup()` を呼んでください
    -   USB: COM/ドライバを確認
    -   BLE: ペアリング済みか、Bluetooth LE が有効か確認
-   USB/BLE の機能が使えない:
    -   `ASControllerSDK.USB.dll` / `ASControllerSDK.BLE.dll` が `.exe` と同じフォルダにあるか確認
    -   `Setup()` を再実行し、`GetConnectedDevices()` の結果を確認

---

<a id="ライセンス"></a>

## ライセンス

[LICENSE](LICENSE.md) を参照してください。
