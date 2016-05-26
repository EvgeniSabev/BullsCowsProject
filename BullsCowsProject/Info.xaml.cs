using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BullsCowsProject
{
    public partial class Info : Window
    {
        internal MainWindow creatingForm { get; set; }

        public Info()
        {
            InitializeComponent();
        }

       private void DevBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DevBox.Text == "devmode")
                {
                    DialogResult = true;
                    this.Close();
                    creatingForm.InfoButton.IsEnabled = false;
               //     creatingForm.TimerStart();
                    creatingForm.inputTextBox.Focus();
                }
            }
        }

        private void RulesLabel_Activate(object sender, MouseButtonEventArgs e)
        {
            var uri = new Uri("pack://application:,,,/Resources/mark.png");
            MarkImage.Source = new BitmapImage(uri);
            DevBox.Focus();
        }
    }
}
