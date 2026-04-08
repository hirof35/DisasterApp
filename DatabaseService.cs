using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DisasterApp
{
    // 備蓄アイテムのデータ構造
    public class StockItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int CurrentQty { get; set; }
        public int TargetQty { get; set; }
        public DateTime ExpiryDate { get; set; }

        [Ignore]
        public bool IsSufficient => CurrentQty >= TargetQty;

        // ★追加：期限が30日以内ならTrue
        [Ignore]
        public bool IsExpiringSoon => (ExpiryDate - DateTime.Now).TotalDays <= 30;
    }

    public class DatabaseService
    {
        private SQLiteAsyncConnection? _db;
        private string _dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "disaster_stock.db3");

        public async Task Init()
        {
            if (_db != null) return;

            _db = new SQLiteAsyncConnection(_dbPath);
            await _db.CreateTableAsync<StockItem>();

            // ★ここが重要：もしデータが0件なら、初期データを入れる
            var count = await _db.Table<StockItem>().CountAsync();
            if (count == 0)
            {
                var initialData = new List<StockItem>
                {
                    new StockItem { Name = "保存水 (2L×6本)", CurrentQty = 2, TargetQty = 6, ExpiryDate = DateTime.Now.AddYears(5) },
                    new StockItem { Name = "非常食 (アルファ米)", CurrentQty = 5, TargetQty = 12, ExpiryDate = DateTime.Now.AddYears(3) },
                    new StockItem { Name = "カセットボンベ", CurrentQty = 1, TargetQty = 3, ExpiryDate = DateTime.Now.AddYears(7) }
                };
                await _db.InsertAllAsync(initialData);
            }
        }

        public async Task<List<StockItem>> GetItems()
        {
            await Init();
            return await _db.Table<StockItem>().ToListAsync();
        }

        // DatabaseServiceクラス内に以下のメソッドを追加
        public async Task UpdateItem(StockItem item)
        {
            await _db!.UpdateAsync(item);
        }

        public async Task AddItem(StockItem item)
        {
            await _db!.InsertAsync(item);
        }
        // DatabaseServiceクラス内に以下のメソッドを追加してください
        public async Task DeleteItem(StockItem item)
        {
            // IDをキーにしてデータベースから削除します
            await _db!.DeleteAsync(item);
        }
    }
}
