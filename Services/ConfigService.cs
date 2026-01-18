using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using cashregister.Model;
using System.Collections.Generic;

namespace cashregister.Services
{
    public class AppState
    {
        public List<CartItem> Cart { get; set; } = new();
    }

    public class ConfigService
    {
        private string GetConfigPath()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            var savedInfoDir = Path.Combine(projectRoot, "SavedInfo");
            if (!Directory.Exists(savedInfoDir)) Directory.CreateDirectory(savedInfoDir);
            return Path.Combine(savedInfoDir, "appstate.json");
        }

        public void SaveState(ObservableCollection<CartItem> cart)
        {
            var state = new AppState { Cart = new List<CartItem>(cart) };
            var json = JsonSerializer.Serialize(state);
            File.WriteAllText(GetConfigPath(), json);
        }

        public void LoadState(ObservableCollection<CartItem> cart)
        {
            var path = GetConfigPath();
            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path);
                    var state = JsonSerializer.Deserialize<AppState>(json);
                    if (state?.Cart != null)
                    {
                        cart.Clear();
                        foreach (var item in state.Cart)
                        {
                            cart.Add(item);
                        }
                    }
                }
                catch { /* ignore errors */ }
            }
        }

        public void ClearState()
        {
            var path = GetConfigPath();
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
