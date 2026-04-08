using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Timers;
namespace DisasterApp
{

    public partial class MainWindow : Window
    {
        private DatabaseService _db = new DatabaseService();
        // 修正前：private Timer _dailyTimer;
        private System.Timers.Timer _dailyTimer;
        public MainWindow()
        {
            InitializeComponent();
            LoadData(); // 起動時にデータを読み込む
            SetupTimer(); // ★定期監視の開始（追加）
        }

        // データを読み込み、リストを更新し、期限をチェックする（一箇所にまとめました）
        private async void LoadData()
        {
            await _db.Init();
            var items = await _db.GetItems();
            StockList.ItemsSource = items;

            // 起動時に期限切れをチェックして通知を出す
            CheckExpirations(items);
        }

        private void CheckExpirations(List<StockItem> items)
        {
            foreach (var item in items)
            {
                // 期限が切れている、または30日以内の場合
                if (item.IsExpiringSoon)
                {
                    string status = (item.ExpiryDate < DateTime.Now) ? "期限切れです！" : "期限が近いです。";

                    // Windowsのトースト通知を送る
                    new ToastContentBuilder()
                        .AddArgument("action", "viewItems")
                        .AddText($"🚨 備蓄アラート: {item.Name}")
                        .AddText($"{item.ExpiryDate:yyyy/MM/dd} に{status} 確認してください。")
                        .Show(); // ★ここが重要！ .Show() で通知を表示します
                }
            }
        }

        private void SearchAmazon_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement el && el.DataContext is StockItem item)
            {
                string url = $"https://www.amazon.co.jp/s?k={WebUtility.UrlEncode(item.Name + " 備蓄用")}";
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
        }

        private void ExportImage_Click(object sender, RoutedEventArgs e)
        {
            if (CaptureArea.ActualWidth == 0) return;

            var bmp = new RenderTargetBitmap((int)CaptureArea.ActualWidth, (int)CaptureArea.ActualHeight, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            bmp.Render(CaptureArea);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "EmergencyCard.png");
            using (var stream = File.Create(path)) { encoder.Save(stream); }

            MessageBox.Show("防災カードをピクチャフォルダに保存しました！");
        }

        private async void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewItemName.Text)) return;

            int.TryParse(NewItemCurrentQty.Text, out int current);
            int.TryParse(NewItemTargetQty.Text, out int target);

            var newItem = new StockItem
            {
                Name = NewItemName.Text,
                CurrentQty = current,
                TargetQty = target > 0 ? target : 1,
                ExpiryDate = DateTime.Now.AddYears(1)
            };

            await _db.AddItem(newItem);

            NewItemName.Text = "";
            NewItemCurrentQty.Text = "0";
            NewItemTargetQty.Text = "3";

            LoadData();
        }

        private async void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is StockItem item)
            {
                if (item.CurrentQty > 0)
                {
                    item.CurrentQty--;
                    await _db.UpdateItem(item);
                    LoadData();
                }
            }
        }

        private async void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is StockItem item)
            {
                item.CurrentQty++;
                await _db.UpdateItem(item);
                LoadData();
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is StockItem item)
            {
                var result = MessageBox.Show($"{item.Name} を削除しますか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _db.DeleteItem(item);
                    LoadData();
                }
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // 終了せずに隠す
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            // 本当に終了するとき
            Application.Current.Shutdown();
        }
        private void SetupTimer()
        {
            _dailyTimer = new System.Timers.Timer(60 * 60 * 1000);

            _dailyTimer.Elapsed += (s, e) => {
                if (DateTime.Now.Hour == 9)
                {
                    Dispatcher.Invoke(() => LoadData());
                }
            };
            _dailyTimer.Start();
        }
    }
}
