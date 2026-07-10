[English](README.md) / [日本語](#jp)

# ASControllerSDK (Windows)

**Version 2.0.0**

**USB** および / または **BLE** 経由で Aroma Shooter デバイスに接続し、制御するための Windows .NET SDK です。

本 SDK は**単一の DLL** として提供されます:

-   `ASControllerSDK.dll` — USB コントローラーと BLE コントローラー、および共通のモデル型を含みます。

もはや Core / プラグインの分割はなく、統合された `AromaShooterController` ファサードもありません。アプリ側で必要なコントローラーを選び、そのコントローラー自身の API を直接呼び出します:

-   `AromaShooterControllerUSB.SharedInstance` — USB / シリアルデバイス用
-   `AromaShooterControllerBLE.SharedInstance` — BLE デバイス用

両コントローラーは同じ形の射出/停止 API（Simple + Intensity）を持ちますが、接続のセットアップ方法だけが異なります（USB のスキャンは同期、BLE のスキャンは非同期です）。

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
│     └─ ASControllerSDK.dll
├─ samples/
│  └─ SmokeTest/
│     ├─ ASControllerSDK.SmokeTest.csproj
│     └─ Program.cs
└─ README.md
```

**重要**: 実行時に `ASControllerSDK.dll` を `.exe` と同じフォルダに置く（または出力フォルダにコピーされる）必要があります。配置が必要な DLL はこの 1 つだけです。USB と BLE の両方の機能がこの中に含まれているため、トランスポートごとに有効/無効を切り替える配置作業はありません。

---

<a id="インストール"></a>

### インストール

#### .NET（デスクトップアプリ）

1. `ASControllerSDK.dll` を参照に追加
2. アプリで必要なコントローラーを利用:
    - USB を使う場合は `AromaShooterControllerUSB.SharedInstance`
    - BLE を使う場合は `AromaShooterControllerBLE.SharedInstance`

`.exe` と同じフォルダに追加で配置する DLL はありません。

#### Unity（Windows）

-   `ASControllerSDK.dll` を Unity プロジェクトへ配置:
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

1. `AromaShooterControllerUSB.SharedInstance.ScanAndConnect()` および/または `await AromaShooterControllerBLE.SharedInstance.ScanAndConnect()` で接続
2. 検出したデバイス一覧を表示
3. Simple の射出 → 停止
4. Intensity の射出 → 停止

---

<a id="使い方"></a>

## 使い方

統合された `AromaShooterController` ファサードはありません。利用したいトランスポートに対応するコントローラーを取得し、そのコントローラー自身のメソッドを呼び出してください。

<a id="0-セットアップ--検出"></a>

### 0. セットアップ / 検出

USB（同期スキャン）:

```csharp
using ASControllerSDK;

var usb = AromaShooterControllerUSB.SharedInstance;
usb.ScanAndConnect();

var usbDevices = usb.GetConnectedDevices(); // 接続中のシリアル番号の List<string>
```

BLE（非同期スキャン — await が必要）:

```csharp
using ASControllerSDK;

var ble = AromaShooterControllerBLE.SharedInstance;
var found = await ble.ScanAndConnect(); // このスキャンで見つかったデバイス名の List<string>

var bleDevices = ble.GetConnectedDevices(); // 接続中のデバイス名の List<string>
```

各コントローラーの `GetConnectedDevices()` は、単純な `List<string>`（デバイスのシリアル/名前）を返します。`{Transport, Identifier, DisplayName}` のようなオブジェクトはありません。どのコントローラーを使ったかで既にトランスポートは分かっているため、両トランスポート間でシリアルを紐づけたい場合はアプリ側で管理してください。

---

<a id="1-simple-射出-api"></a>

### 1. Simple 射出 API

`AromaShooterControllerUSB.SharedInstance` と `AromaShooterControllerBLE.SharedInstance` の両方に、同名のメソッドとして用意されています（以下の例では `usb` を使用していますが、BLE デバイスの場合は `ble` に置き換えてください）:

全デバイスへ射出:

```csharp
usb.ShootAllSimple(3000, new int[] { 1, 2, 5 }, internalBooster: true);
```

特定デバイスへ射出:

```csharp
usb.ShootSimple(3000, new int[] { 1, 2, 5 }, true, "ASN3A01192");
```

全停止:

```csharp
usb.StopAllSimple();
```

特定デバイス停止:

```csharp
usb.StopSimple("ASN3A01192");
```

---

<a id="2-intensity-射出-api"></a>

### 2. Intensity 射出 API

`AromaChamber`:

```csharp
public class AromaChamber
{
    public int number;        // 1..6
    public int concentration; // 0..100
}
```

強度制御ありで全デバイスへ射出:

```csharp
var chambers = new List<AromaChamber>
{
    new AromaChamber { number = 1, concentration = 50 },
    new AromaChamber { number = 2, concentration = 80 },
};

usb.ShootAllWithIntensity(
    3000,
    chambers,
    internalBoosterIntensity: 100,
    externalBoosterIntensity: 0
);
```

強度制御ありで特定デバイスへ射出:

```csharp
usb.ShootWithIntensity(
    3000,
    chambers,
    internalBoosterIntensity: 100,
    externalBoosterIntensity: 0,
    shooterName: "ASN3A01192"
);
```

強度制御の停止（チャンバー指定）:

```csharp
usb.StopAllWithIntensity(
    chambers: new[] { 1, 2 },
    stopInternalBooster: true,
    stopExternalBooster: false
);
```

特定デバイスの強度制御停止（チャンバー指定）:

```csharp
usb.StopWithIntensity(
    "ASN3A01192",
    chambers: new[] { 1, 2 },
    stopInternalBooster: true,
    stopExternalBooster: false
);
```

`AromaShooterControllerBLE.SharedInstance` も同じ Intensity API（`ShootAllWithIntensity` / `ShootWithIntensity` / `StopAllWithIntensity` / `StopWithIntensity`）を持ちます。BLE デバイスも USB デバイスと同様に強度制御に対応しています。

---

<a id="トラブルシューティング"></a>

## トラブルシューティング

-   デバイスが見つからない:
    -   USB: 先に `usb.ScanAndConnect()` を呼び、COM ポート/ドライバを確認
    -   BLE: 先に `await ble.ScanAndConnect()` を呼び、ペアリング済みか、Bluetooth LE が有効か確認
-   コントローラーの選び間違い:
    -   USB と BLE は別々のコントローラー（`AromaShooterControllerUSB`、`AromaShooterControllerBLE`）で扱われます。デバイスの接続方式に合ったコントローラーを使用しているか確認してください
    -   `ScanAndConnect()` を再実行し、`GetConnectedDevices()` の結果を確認

---

<a id="ライセンス"></a>

## ライセンス

[LICENSE](LICENSE.md) を参照してください。
