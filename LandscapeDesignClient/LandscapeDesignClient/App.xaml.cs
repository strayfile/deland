using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LandscapeDesignClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static List<CultureInfo> _languages = new List<CultureInfo>(); //список языков
        public static event EventHandler LanguageChanged; //событие изменения языка

        public App()
        {
            InitializeComponent();
            App.LanguageChanged += App_LanguageChanged;
            _languages.Clear();
            _languages.Add(new CultureInfo("ru-RU")); //Русская - нейтральная культура
            _languages.Add(new CultureInfo("en-US")); //Английская
            Language = LandscapeDesignClient.Properties.Settings.Default.DefaultLanguage;
            
        }
        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Language = LandscapeDesignClient.Properties.Settings.Default.DefaultLanguage;
        }
        private void App_LanguageChanged(Object sender, EventArgs e)
        {
            LandscapeDesignClient.Properties.Settings.Default.DefaultLanguage = Language;
            LandscapeDesignClient.Properties.Settings.Default.Save();
        }
        
        public static CultureInfo Language
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (value == System.Threading.Thread.CurrentThread.CurrentUICulture)
                    return;
                //Меняем язык приложения
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;
                //Создаём ResourceDictionary для новой культуры
                ResourceDictionary dict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "en-US":
                        dict.Source = new Uri($"Resources/lang.{value.Name}.xaml", UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri("Resources/lang.xaml", UriKind.Relative);
                        break;
                }
                //Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
                                              select d).First();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else Application.Current.Resources.MergedDictionaries.Add(dict);
                //Вызываем евент для оповещения всех окон
                LanguageChanged(Application.Current, new EventArgs());
            }
        }
        public static List<CultureInfo> Languages
        {
            get
            {
                return _languages;
            }
        }
    }
}
