using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BullsCowsProject
{
    public partial class MainWindow : Window
    {
        internal MainWindow creatingForm { get; set; }

       
        public static int moves = 0;
        static int[] number = new int[4];

        public MainWindow()
        {
            InitializeComponent();
            ScoreBoard.LoadScoreBoard();
            Loaded += MyWindow_Loaded;
        }

        //This method will will called when the main window is loaded
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            moves = 0;
            inputTextBox.MaxLength = 4;
            GenerateNumber();
            inputTextBox.Focus();
        }

        //This method generates the random number, with 4 different digits
        void GenerateNumber()
        {
            for (int i = 0; i < number.Length; i++)
            {
                number[i] = GenerateRandomDigit();
                //MessageBox.Show("digit" + digit);
            }
            randomNumLabel.Content = string.Join("", number);
        }

       
        private int GenerateRandomDigit()
        {
            Random randomizer = new Random();
            int digit;
            do
            {
                digit = randomizer.Next(1, 9);

            }
            while (number.Contains(digit));

            return digit;

        }

       
        private void inputTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
           
            IsNumeric(e);
         
            IsUnique(e);
        }

        private void IsNumeric(TextCompositionEventArgs e)
        {
            int result;

            if (!(int.TryParse(e.Text, out result)))
            {
                e.Handled = true;
            }
            else if (int.TryParse(e.Text, out result))
            {
                if (result == 0)
                {
                    e.Handled = true;
                }
            }
        }

        private void IsUnique(TextCompositionEventArgs e)
        {
            int input;
            if (int.TryParse(e.Text, out input))
            {
                int[] previousInput = inputTextBox.Text.ToCharArray().Select(d => Convert.ToInt32(d) - 48).ToArray();

                //check if the entered char exists in the textbox
                if (previousInput.Contains(input))
                {
                    e.Handled = true;
                }
            }
        }

       
        private void inputTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Exposes Button types to UI Automation.
                ButtonAutomationPeer peer = new ButtonAutomationPeer(checkButton);

                // IInvokeProvider Interface - Exposes methods and properties to support UI Automation client access to controls that initiate or perform a single, unambiguous action and do not maintain state when activated. 
                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;

                // Invoke() - Sends a request to activate a control and initiate its single, unambiguous action.
                invokeProv.Invoke();
            }
            else if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }


        //disable pasting
        private void inputTextBox_TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }


        void checkButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputTextBox.Text.Length != 4)
            {
                MessageBox.Show("The input must be 4 digits");
            }
            else
            {
                //remove all images from the array with all bulls and cows placeholders
                Image[] imgArray = new Image[] { bull, bull2, bull3, bull4, cow, cow2, cow3, cow4 };
                for (int i = 0; i < imgArray.Length; i++)
                {
                    imgArray[i].Source = null;
                }

                GetValueFromTextBox();
            }
            inputTextBox.Clear();
        }

        void GetValueFromTextBox()
        {
            string playerNumber = inputTextBox.Text;
            int[] playerDigits = playerNumber.ToCharArray().Select(d => Convert.ToInt32(d) - 48).ToArray();

            FindBullsCows(playerDigits);
        }

        // Main part of the program 
        void FindBullsCows(int[] playerDigits)
        {
            int bulls = 0;
            int cows = 0;

            //iterate through the user input
            for (int i = 0; i < playerDigits.Length; i++)
            {
                //check if the current digit is contained into the random number
                if (number.Contains(playerDigits[i]))
                {
                    // If its contained, then check if its in the same position as in the original number, and add bull or cow accordingly  
                    if (number[i] == playerDigits[i])
                    {
                        bulls++;
                    }
                    else
                    {
                        cows++;
                    }
                }
            }
            moves++;

            DrawCows(cows);
            DrawBulls(bulls);
            AddHistory(string.Join("", playerDigits), bulls, cows);

            if (bulls == 4)
            {
                DrawCows(4);
                
                Congratulations victory = new Congratulations(moves);
                victory.creatingForm = this;
                
                victory.ShowDialog();
            }

        }

        private void AddHistory(string playerNumber, int bulls, int cows)
        {
            ListBoxItem history = new ListBoxItem();
            if (bulls == 4)
            {
                history.Content = ("The number was " + playerNumber + ". You won in " + moves + " moves!");
            }
            else
            {
                history.Content = "In " + playerNumber + " there are " + bulls + " bulls and " + cows + " cows";
            }

            historyListBox.Items.Insert(0, history);
        }

        void DrawBulls(int bulls)
        {
           
            Image[] imgArray = new Image[] { bull, bull2, bull3, bull4 };
            for (int i = 0; i < bulls; i++)
            {
                var uri = new Uri("pack://application:,,,/Resources/bull.png");
                imgArray[i].Source = new BitmapImage(uri);
            }
        }

        void DrawCows(int cows)
        {
            Image[] imgArray = new Image[] { cow, cow2, cow3, cow4 };
            for (int i = 0; i < cows; i++)
            {
                var uri = new Uri("pack://application:,,,/Resources/cow.png");
                imgArray[i].Source = new BitmapImage(uri);
            }
        }

        
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            Info helpWindow = new Info();
            helpWindow.creatingForm = this;
            helpWindow.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Statistics statisticsWindow = new Statistics();
            //   statisticsWindow.creatingForm = this;
            statisticsWindow.ShowDialog();
        }

        //Everything below is code for the animations

        //public void TimerStart()
        //{
        //    DispatcherTimer dispatcherTimer = new DispatcherTimer();
        //    dispatcherTimer.Tick += CallAnimations;
        //    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);

        //    dispatcherTimer.Start();
        //}
/*
        private void CallAnimations(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();
            AnimateHistoryTab();
            AnimateHitoryLabel();r
            AnimateCheckButton();
            AnimateRandomNumber();
            AnimateInputBox();
            AnimateFirstCow();
            AnimateSecondCow();
            AnimateThirdCow();
            AnimateForthCow();
            AnimateFirstBull();
            AnimateSecondBull();
            AnimateThirdBull();
            AnimateForthBull();
            AnimateTitle();
            AnimateHelp();
        }

        private void secondAnimationInfoButtonStart(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            double oldLeft = InfoButton.Margin.Left;
            double oldRight = InfoButton.Margin.Right;
            double oldTop = InfoButton.Margin.Top;
            double oldBottom = InfoButton.Margin.Bottom;

            InfoButton.Margin = new Thickness(oldLeft, oldTop + 120, oldRight, oldBottom);

            DoubleAnimation rotate = new DoubleAnimation();
            rotate.From = 0;
            rotate.To = -5;
            rotate.AutoReverse = false;
            rotate.Duration = new Duration(TimeSpan.FromSeconds(0.250));

            RotateTransform rt = new RotateTransform();
            InfoButton.RenderTransform = rt;
            rt.BeginAnimation(RotateTransform.AngleProperty, rotate);
        }

        private void AnimateHelp()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 120;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(1.750));

            TranslateTransform tt = new TranslateTransform();
            InfoButton.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);

            DispatcherTimer secondAnimationInfoButton = new DispatcherTimer();
            secondAnimationInfoButton.Tick += secondAnimationInfoButtonStart;
            secondAnimationInfoButton.Interval = new TimeSpan(0, 0, 0, 1, 750);

            secondAnimationInfoButton.Start();
        }

        private void AnimateTitle()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 115;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(1.500));

            TranslateTransform tt = new TranslateTransform();
            TitleLabel.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);
        }

        private void ThirdAnimationForthBullStart(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            double oldLeft = bull4.Margin.Left;
            double oldRight = bull4.Margin.Right;
            double oldTop = bull4.Margin.Top;
            double oldBottom = bull4.Margin.Bottom;

            bull4.RenderTransformOrigin = new Point(0, 0.5);

            bull4.Margin = new Thickness(oldLeft - 13, oldTop + 48, oldRight + 13, oldBottom);

            DoubleAnimation rotate = new DoubleAnimation();
            rotate.From = -45;
            rotate.To = -30;
            rotate.AutoReverse = false;
            rotate.Duration = new Duration(TimeSpan.FromSeconds(0.250));

            RotateTransform rt = new RotateTransform();

            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 7;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.250));

            TranslateTransform tt = new TranslateTransform();

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(rt);
            tg.Children.Add(tt);

            bull4.RenderTransform = tg;

            rt.BeginAnimation(RotateTransform.AngleProperty, rotate);
            tt.BeginAnimation(TranslateTransform.YProperty, translate);
        }

        private void secondAnimationForthBullStart(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            double oldLeft = bull4.Margin.Left;
            double oldRight = bull4.Margin.Right;
            double oldTop = bull4.Margin.Top;
            double oldBottom = bull4.Margin.Bottom;

            bull4.Margin = new Thickness(oldLeft, oldTop + 85, oldRight, oldBottom);

            DoubleAnimation rotate = new DoubleAnimation();
            rotate.From = 0;
            rotate.To = -45;
            rotate.AutoReverse = false;
            rotate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            RotateTransform rt = new RotateTransform();

            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 20;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            TranslateTransform tt = new TranslateTransform();

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(rt);
            tg.Children.Add(tt);

            bull4.RenderTransform = tg;

            rt.BeginAnimation(RotateTransform.AngleProperty, rotate);
            tt.BeginAnimation(TranslateTransform.YProperty, translate);

            DispatcherTimer ThirdAnimationForthBull = new DispatcherTimer();
            ThirdAnimationForthBull.Tick += ThirdAnimationForthBullStart;
            ThirdAnimationForthBull.Interval = new TimeSpan(0, 0, 0, 0, 500);

            ThirdAnimationForthBull.Start();
        }

        private void AnimateForthBull()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 85;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(1.200));

            TranslateTransform tt = new TranslateTransform();
            bull4.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);

            DispatcherTimer secondAnimationForthBull = new DispatcherTimer();
            secondAnimationForthBull.Tick += secondAnimationForthBullStart;
            secondAnimationForthBull.Interval = new TimeSpan(0, 0, 0, 1, 200);

            secondAnimationForthBull.Start();
        }

        private void secondAnimationThirdBullStart(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            double oldLeft = bull3.Margin.Left;
            double oldRight = bull3.Margin.Right;
            double oldTop = bull3.Margin.Top;
            double oldBottom = bull3.Margin.Bottom;

            bull3.Margin = new Thickness(oldLeft, oldTop + 100, oldRight, oldBottom);

            DoubleAnimation rotate = new DoubleAnimation();
            rotate.From = 0;
            rotate.To = 55;
            rotate.AutoReverse = false;
            rotate.Duration = new Duration(TimeSpan.FromSeconds(0.750));

            RotateTransform rt = new RotateTransform();

            DoubleAnimation translateY = new DoubleAnimation();
            translateY.From = 0;
            translateY.To = 18;
            translateY.AutoReverse = false;
            translateY.Duration = new Duration(TimeSpan.FromSeconds(0.750));

            DoubleAnimation translateX = new DoubleAnimation();
            translateX.From = 0;
            translateX.To = 30;
            translateX.AutoReverse = false;
            translateX.Duration = new Duration(TimeSpan.FromSeconds(0.750));

            TranslateTransform tt = new TranslateTransform();

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(rt);
            tg.Children.Add(tt);


            bull3.RenderTransform = tg;
            rt.BeginAnimation(RotateTransform.AngleProperty, rotate);
            tt.BeginAnimation(TranslateTransform.XProperty, translateX);
            tt.BeginAnimation(TranslateTransform.YProperty, translateY);
        }

        private void AnimateThirdBull()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 100;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(1.100));

            TranslateTransform tt = new TranslateTransform();
            bull3.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);

            DispatcherTimer secondAnimationThirdBull = new DispatcherTimer();
            secondAnimationThirdBull.Tick += secondAnimationThirdBullStart;
            secondAnimationThirdBull.Interval = new TimeSpan(0, 0, 0, 1, 100);

            secondAnimationThirdBull.Start();
        }

        private void secondAnimationSecondBullStart(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            double oldLeft = bull2.Margin.Left;
            double oldRight = bull2.Margin.Right;
            double oldTop = bull2.Margin.Top;
            double oldBottom = bull2.Margin.Bottom;

            bull2.Margin = new Thickness(oldLeft, oldTop + 60, oldRight, oldBottom);

            DoubleAnimation rotate = new DoubleAnimation();
            rotate.From = 0;
            rotate.To = -45;
            rotate.AutoReverse = false;
            rotate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            RotateTransform rt = new RotateTransform();

            DoubleAnimation translateY = new DoubleAnimation();
            translateY.From = 0;
            translateY.To = 20;
            translateY.AutoReverse = false;
            translateY.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            DoubleAnimation TranslateX = new DoubleAnimation();
            TranslateX.From = 0;
            TranslateX.To = -8;
            TranslateX.AutoReverse = false;
            TranslateX.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            TranslateTransform tt = new TranslateTransform();

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(rt);
            tg.Children.Add(tt);


            bull2.RenderTransform = tg;
            rt.BeginAnimation(RotateTransform.AngleProperty, rotate);
            tt.BeginAnimation(TranslateTransform.XProperty, TranslateX);
            tt.BeginAnimation(TranslateTransform.YProperty, translateY);
        }

        private void AnimateSecondBull()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 60;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.900));

            TranslateTransform tt = new TranslateTransform();
            bull2.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);

            DispatcherTimer secondAnimationSecondBull = new DispatcherTimer();
            secondAnimationSecondBull.Tick += secondAnimationSecondBullStart;
            secondAnimationSecondBull.Interval = new TimeSpan(0, 0, 0, 0, 900);

            secondAnimationSecondBull.Start();
        }

        private void AnimateFirstBull()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 110;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(1.300));

            TranslateTransform tt = new TranslateTransform();
            bull.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);
        }

        private void secondAnimationForthCowStart(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            double oldLeft = cow4.Margin.Left;
            double oldRight = cow4.Margin.Right;
            double oldTop = cow4.Margin.Top;
            double oldBottom = cow4.Margin.Bottom;

            cow4.Margin = new Thickness(oldLeft, oldTop + 45, oldRight, oldBottom);

            DoubleAnimation rotate = new DoubleAnimation();
            rotate.From = 0;
            rotate.To = 45;
            rotate.AutoReverse = false;
            rotate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            RotateTransform rt = new RotateTransform();

            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = -2;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(1.000));

            TranslateTransform tt = new TranslateTransform();

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(rt);
            tg.Children.Add(tt);

            cow4.RenderTransform = tg;
            rt.BeginAnimation(RotateTransform.AngleProperty, rotate);
            tt.BeginAnimation(TranslateTransform.YProperty, translate);

        }

        private void AnimateForthCow()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 50;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            TranslateTransform tt = new TranslateTransform();
            cow4.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);

            DispatcherTimer secondAnimationForthCow = new DispatcherTimer();
            secondAnimationForthCow.Tick += secondAnimationForthCowStart;
            secondAnimationForthCow.Interval = new TimeSpan(0, 0, 0, 0, 500);

            secondAnimationForthCow.Start();
        }

        private void secondAnimationThirdCowStart(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            double oldLeft = cow3.Margin.Left;
            double oldRight = cow3.Margin.Right;
            double oldTop = cow3.Margin.Top;
            double oldBottom = cow3.Margin.Bottom;

            cow3.Margin = new Thickness(oldLeft, oldTop + 50, oldRight, oldBottom);

            DoubleAnimation rotate = new DoubleAnimation();
            rotate.From = 0;
            rotate.To = -36;
            rotate.AutoReverse = false;
            rotate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            RotateTransform rt = new RotateTransform();
            cow3.RenderTransform = rt;
            rt.BeginAnimation(RotateTransform.AngleProperty, rotate);
        }

        private void AnimateThirdCow()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 50;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            TranslateTransform tt = new TranslateTransform();
            cow3.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);

            DispatcherTimer secondAnimationThirdCow = new DispatcherTimer();
            secondAnimationThirdCow.Tick += secondAnimationThirdCowStart;
            secondAnimationThirdCow.Interval = new TimeSpan(0, 0, 0, 0, 500);

            secondAnimationThirdCow.Start();
        }

        private void secondAnimationSecondCowStart(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            double oldLeft = cow2.Margin.Left;
            double oldRight = cow2.Margin.Right;
            double oldTop = cow2.Margin.Top;
            double oldBottom = cow2.Margin.Bottom;

            cow2.Margin = new Thickness(oldLeft, oldTop + 37, oldRight, oldBottom);

            DoubleAnimation rotate = new DoubleAnimation();
            rotate.From = 0;
            rotate.To = 24;
            rotate.AutoReverse = false;
            rotate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            RotateTransform rt = new RotateTransform();
            cow2.RenderTransform = rt;
            rt.BeginAnimation(RotateTransform.AngleProperty, rotate);
        }

        private void AnimateSecondCow()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 37;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            TranslateTransform tt = new TranslateTransform();
            cow2.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);

            DispatcherTimer secondAnimationSecondCow = new DispatcherTimer();
            secondAnimationSecondCow.Tick += secondAnimationSecondCowStart;
            secondAnimationSecondCow.Interval = new TimeSpan(0, 0, 0, 0, 500);

            secondAnimationSecondCow.Start();
        }

        private void secondAnimationFirstCowStart(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            double oldLeft = cow.Margin.Left;
            double oldRight = cow.Margin.Right;
            double oldTop = cow.Margin.Top;
            double oldBottom = cow.Margin.Bottom;

            cow.Margin = new Thickness(oldLeft, oldTop + 35, oldRight, oldBottom);

            DoubleAnimation rotate = new DoubleAnimation();
            rotate.From = 0;
            rotate.To = -80;
            rotate.AutoReverse = false;
            rotate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            RotateTransform rt = new RotateTransform();
            cow.RenderTransform = rt;
            rt.BeginAnimation(RotateTransform.AngleProperty, rotate);
        }

        private void AnimateFirstCow()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 35;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            TranslateTransform tt = new TranslateTransform();
            cow.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);

            DispatcherTimer secondAnimationFirstCow = new DispatcherTimer();
            secondAnimationFirstCow.Tick += secondAnimationFirstCowStart;
            secondAnimationFirstCow.Interval = new TimeSpan(0, 0, 0, 0, 500);

            secondAnimationFirstCow.Start();
        }

        private void secondAnimationInputBoxStart(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            double oldLeft = inputTextBox.Margin.Left;
            double oldRight = inputTextBox.Margin.Right;
            double oldTop = inputTextBox.Margin.Top;
            double oldBottom = inputTextBox.Margin.Bottom;

            inputTextBox.Margin = new Thickness(oldLeft, oldTop + 76, oldRight, oldBottom);

            DoubleAnimation rotate = new DoubleAnimation();
            rotate.From = 0;
            rotate.To = -5;
            rotate.AutoReverse = false;
            rotate.Duration = new Duration(TimeSpan.FromSeconds(0.200));

            RotateTransform rt = new RotateTransform();
            inputTextBox.RenderTransform = rt;
            rt.BeginAnimation(RotateTransform.AngleProperty, rotate);
        }

        private void AnimateInputBox()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 76;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.750));

            TranslateTransform tt = new TranslateTransform();
            inputTextBox.RenderTransform = tt;

            tt.BeginAnimation(TranslateTransform.YProperty, translate);

            DispatcherTimer secondAnimationInputBox = new DispatcherTimer();
            secondAnimationInputBox.Tick += secondAnimationInputBoxStart;
            secondAnimationInputBox.Interval = new TimeSpan(0, 0, 0, 0, 750);

            secondAnimationInputBox.Start();
        }

        private void AnimateRandomNumber()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 46;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.750));

            TranslateTransform tt = new TranslateTransform();
            randomNumLabel.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);
        }

        private void AnimateCheckButton()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 68;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.750));

            TranslateTransform tt = new TranslateTransform();
            checkButton.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);
        }

        private void AnimateHitoryLabel()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 20;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            TranslateTransform tt = new TranslateTransform();
            HistoryLabel.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);
        }

        private void AnimateHistoryTab()
        {
            DoubleAnimation translate = new DoubleAnimation();
            translate.From = 0;
            translate.To = 10;
            translate.AutoReverse = false;
            translate.Duration = new Duration(TimeSpan.FromSeconds(0.500));

            TranslateTransform tt = new TranslateTransform();
            historyListBox.RenderTransform = tt;
            tt.BeginAnimation(TranslateTransform.YProperty, translate);
        }
*/
        /*     private void newGameButton_Click(object sender, RoutedEventArgs e)
             {
                 MainWindow newGame = new MainWindow();
                 newGame.Show();
                // creatingForm.Close();
           
            } */
    }
}