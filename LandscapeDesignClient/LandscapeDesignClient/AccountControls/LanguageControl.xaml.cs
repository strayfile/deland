using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;

namespace LandscapeDesignClient.AccountControls
{
    public partial class LanguageControl : UserControl
    {
        public LanguageControl()
        {
            InitializeComponent();
            Languages = new ObservableCollection<MenuItem>();
            cmbLanguage.DataContext = this;
            App.LanguageChanged += LanguageChanged;
            SetLanguageControl();
        }
        public void SetLanguageControl()
        {
            Languages = new ObservableCollection<MenuItem>();
            CultureInfo currLang = App.Language;
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem
                {
                    Header = lang.DisplayName,
                    Tag = lang,
                    IsChecked = lang.Equals(currLang)
                };
                menuLang.Click += ChangeLanguageClick;
                Languages.Add(menuLang);
            }
        }
        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;
            foreach (MenuItem i in Languages)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }

        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            if (sender is MenuItem mi)
                if (mi.Tag is CultureInfo lang)
                    App.Language = lang;
        }

        public ObservableCollection<MenuItem> Languages { get; private set; }
    }

    //public partial class CopyOfLanguageControl : UserControl
    //{
    //    public CopyOfLanguageControl()
    //    {
    //        InitializeComponent();
    //        Languages = new ObservableCollection<MenuItem>();
    //        cmbLanguage.DataContext = this;
    //        App.LanguageChanged += LanguageChanged;
    //        SetLanguageControl();
    //    }
    //    public void SetLanguageControl()
    //    {
    //        Languages = new ObservableCollection<MenuItem>();
    //        CultureInfo currLang = App.Language;
    //        foreach (var lang in App.Languages)
    //        {
    //            MenuItem menuLang = new MenuItem
    //            {
    //                Header = lang.DisplayName,
    //                Tag = lang,
    //                IsChecked = lang.Equals(currLang)
    //            };
    //            menuLang.Click += ChangeLanguageClick;
    //            Languages.Add(menuLang);
    //        }
    //    }
    //    private void LanguageChanged(Object sender, EventArgs e)
    //    {
    //        CultureInfo currLang = App.Language;
    //        foreach (MenuItem i in Languages)
    //        {
    //            CultureInfo ci = i.Tag as CultureInfo;
    //            i.IsChecked = ci != null && ci.Equals(currLang);
    //        }
    //    }

    //    private void ChangeLanguageClick(Object sender, EventArgs e)
    //    {
    //        if (sender is MenuItem mi)
    //            if (mi.Tag is CultureInfo lang)
    //                App.Language = lang;
    //    }

    //    public ObservableCollection<MenuItem> Languages { get; private set; }
    //}
}
