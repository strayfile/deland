using System;
using System.Windows;
using LandscapeDesignServer.LandscapeDesignModel;
using System.ServiceModel;

namespace LandscapeDesignServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceHost sh = null;
        public MainWindow()
        {
            InitializeComponent();
            LogMessages logs = LogMessages.GetInstance();
            lvLogs.ItemsSource = logs.Logs;
            var r = typeof(LandscapeDesignService);
            try
            {
                ModelFactory.GetInstance();
                logs.Add("Подключение к бд успешно.");
                try
                {
                    sh = new ServiceHost(r);
                    sh.Open();
                    logs.Add("Сервис запущен.");
                }
                catch (Exception)
                {
                    logs.Add("Выполните запуск с правами администратора.");
                }
            }
            catch (Exception)
            {
                logs.Add("Ошибка при подключении к бд.");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sh != null)
            {
                if (sh.State != CommunicationState.Closed && sh.State != CommunicationState.Faulted)
                    sh.Close();
            }
        }
    }
}
