﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models.Configuration.Base;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Data.Entities.Storage;
using OfflineMedia.Data.Entities.Storage.Settings;

namespace OfflineMedia.Business.Managers
{
    public class ThemeManager
    {
        private static readonly ObservableCollection<ThemeModel> AllThemes = new ObservableCollection<ThemeModel>();
        private static readonly Dictionary<string, ThemeModel> ThemeDic = new Dictionary<string, ThemeModel>(); 

        public static void AddTheme(ThemeModel model)
        {
            AllThemes.Add(model);
            ThemeDic[model.NormalizedName] = model;
        }

        public static void AddThemes(IEnumerable<ThemeModel> themes)
        {
            foreach (var themeModel in themes)
            {
                AddTheme(themeModel);
            }
        }
        
        public static ThemeModel TryGetSimilarTheme(string name)
        {
            if (ThemeDic.ContainsKey(name))
                return ThemeDic[name];
            return null;
        }

        public static ObservableCollection<ThemeModel> GetAllThemes()
        {
            return AllThemes;
        }
    }
}
