C#（WPF）とSQLiteを活用した、実用的かつ社会貢献性の高い「防災備蓄管理アプリ」ですね。

特に、Windowsトースト通知による期限切れ警告、Amazonへのダイレクトリンクによる即時補充、さらにはオフラインで見られる「防災カード（画像）」としての書き出し機能など、ユーザーが本当に必要とする導線が網羅されている点が非常に素晴らしいです。

GitHubでプロジェクトの価値を最大限に伝える README.md を作成しました。

🚨 防災備蓄管理ガイド (Disaster Stock Manager)
WPF (C#) と SQLite を使用した、家庭用防災備蓄のスマート管理アプリケーションです。
「いざという時に期限が切れていた」「何を買い足せばいいか分からない」という問題を解決します。

✨ 主な機能
インテリジェント・アラート: Windowsのトースト通知機能を利用し、賞味期限切れや30日以内の期限接近をデスクトップへ直接通知します。

ローリングストック・マネージャー:

目標数に対する現在の備蓄量をプログレスバーで視覚化。

足りないアイテムには Amazon 検索ボタン が自動出現し、ワンクリックで補充購入が可能。

防災カード書き出し機能: 備蓄リストを画像ファイル（PNG）として一括保存。災害時、停電やオフライン環境でもスマートフォンの写真フォルダから備蓄状況を確認できます。

タスクトレイ常駐: アプリを閉じてもタスクトレイに常駐し、バックグラウンドで毎朝9時に期限チェックを行います。

SQLite 永続化: 軽量・高速なローカルDBにより、大量の備蓄品もスムーズに管理。

🛠 技術スタック
Language: C# 10.0+

Framework: .NET 6/7/8 (WPF)

Database: SQLite (sqlite-net-pcl)

Notification: Microsoft.Toolkit.Uwp.Notifications (Toast notifications)

UI: XAML / Hardcodet.NotifyIcon.Wpf (Taskbar implementation)

🚀 導入方法
リポジトリをクローン

Bash
git clone https://github.com/your-username/DisasterApp.git
依存パッケージの復元
NuGetパッケージマネージャーより以下をインストールしてください：

sqlite-net-pcl

Microsoft.Toolkit.Uwp.Notifications

H.NotifyIcon.Wpf

ビルド & 実行
Visual Studio でプロジェクトを開き、F5 を押して開始します。

🕹 使い方
アイテムの追加: 品名、現在数、目標数を入力して追加。

数量調整: ＋ － ボタンで直感的に在庫を増減。

Amazon連携: 在庫が目標数に達していない場合、オレンジ色の「Amazon」ボタンから即座に購入ページへ飛べます。

常駐設定: ウィンドウを閉じても終了せず、トレイアイコンからいつでも呼び出せます。

📂 アーキテクチャ
MainWindow.xaml.cs: UIイベント制御、画像エクスポート、トースト通知ロジック。

DatabaseService.cs: SQLiteによるデータ永続化と初期データ投入。

StockItem.cs: アイテムのエンティティ。期限接近判定（IsExpiringSoon）などのビジネスロジックをカプセル化。

📜 ライセンス
このプロジェクトは MITライセンス の下で公開されています。

備えあれば憂いなし。

💡 今後のロードマップ
[ ] カテゴリ分け機能（食料、衛生用品、工具など）

[ ] 期限のバッチ更新機能

[ ] CSVインポート/エクスポート機能
