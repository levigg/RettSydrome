using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Forms;
using System.IO;
using NAudio.Wave;
using System.Diagnostics;
using System.ComponentModel;
using System.Speech.Synthesis;

namespace RettSydrome
{
    public partial class EditorWindow : Window, INotifyPropertyChanged
    {
        #region Initial
        public EyeCanvas EC;
        MainWindow homeWindow;

        System.Windows.Threading.DispatcherTimer timer;

        int gazeTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TorFGaze"]);
        int leaveTime = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TorFLeave"]);
        MediaPlayer soundPlayer_play;
        MediaPlayer soundPlayer_edit;

        double screenHeight = Screen.PrimaryScreen.Bounds.Height;
        double screenWidth = Screen.PrimaryScreen.Bounds.Width;

        string mode = "editor";

        string path_1246;
        string path_subject;
        string path_question;

        int ER = 12345;

        MediaElement[] video_array;
        Image[] image_array;
        System.Windows.Controls.Button[] edit_button_array;
        System.Windows.Controls.Button[] load_video_button_array;
        System.Windows.Controls.Button[] load_image_button_array;
        System.Windows.Controls.Button[] sound_button_array;
        System.Windows.Controls.Button[] delete1246_button_array;
        System.Windows.Controls.StackPanel[] choose_sound_box_array;
        System.Windows.Shapes.Rectangle[] hover_border_array;
        System.Windows.Controls.Button sound_button_ER;
        System.Windows.Controls.Button delete_button_ER;
        System.Windows.Controls.StackPanel soundBox_ER;
        System.Windows.Controls.StackPanel stackPnl;
        System.Windows.Controls.Button choose_sound_button_ER;
        System.Windows.Controls.Button record_sound_button_ER;
        System.Windows.Controls.Button TTS_sound_button_ER;
        System.Windows.Controls.Image recordBg_ER;
        System.Windows.Controls.Border text_ER;
        System.Windows.Controls.TextBox textbox_ER;
        //System.Windows.Shapes.Rectangle dragRect_ER;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;


        public EditorWindow()
        {

            InitializeComponent();
            DataContext = this;

            sound_button_ER = new System.Windows.Controls.Button() { Width = 30, Height = 30 };
            sound_button_ER.Style = (Style)TryFindResource("style_no_sound");
            sound_button_ER.Click += new RoutedEventHandler(sound_button_ER_Click);
            delete_button_ER = new System.Windows.Controls.Button() { Width = 30, Height = 30 };
            delete_button_ER.Style = (Style)TryFindResource("style_delete1246");
            delete_button_ER.Click += new RoutedEventHandler(delete_button_ER_Click);

            recordBg_ER = new System.Windows.Controls.Image();
            recordBg_ER.Style = (Style)TryFindResource("style_recordBg_1");
            choose_sound_button_ER = new System.Windows.Controls.Button();
            choose_sound_button_ER.Style = (Style)TryFindResource("styleOrangeBtn1");
            choose_sound_button_ER.Content = TryFindResource("strChooseSound");
            choose_sound_button_ER.Click += new RoutedEventHandler(choose_sound_button_Click);
            record_sound_button_ER = new System.Windows.Controls.Button();
            record_sound_button_ER.Style = (Style)TryFindResource("styleOrangeBtn2");
            record_sound_button_ER.Content = TryFindResource("strRecordSound");
            record_sound_button_ER.Click += new RoutedEventHandler(record_sound_button_Click);
            TTS_sound_button_ER = new System.Windows.Controls.Button();
            TTS_sound_button_ER.Style = (Style)TryFindResource("styleOrangeBtn1");
            TTS_sound_button_ER.Content = TryFindResource("strTTSSound");
            TTS_sound_button_ER.Click += new RoutedEventHandler(TTS_sound_button_Click);

            stackPnl = new StackPanel();
            stackPnl.Orientation = System.Windows.Controls.Orientation.Horizontal;
            stackPnl.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            Thickness margin = stackPnl.Margin;
            margin.Top = 20;
            stackPnl.Margin = margin;
            stackPnl.Children.Add(choose_sound_button_ER);
            stackPnl.Children.Add(record_sound_button_ER);
            stackPnl.Children.Add(TTS_sound_button_ER);

            soundBox_ER = new StackPanel();
            soundBox_ER.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            soundBox_ER.Children.Add(recordBg_ER);
            soundBox_ER.Children.Add(stackPnl);

            textbox_ER = new System.Windows.Controls.TextBox();
            textbox_ER.Style = (Style)TryFindResource("style_ER_textBox");

            text_ER = new Border();
            text_ER.Style = (Style)TryFindResource("style_ER_text");
            text_ER.Child = textbox_ER;

            //dragRect_ER = new Rectangle();
            //dragRect_ER.Style = (Style)TryFindResource("style_ER_dragRect");
            //dragRect_ER.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 203, 139));
            //dragRect_ER.MouseLeftButtonDown += new MouseButtonEventHandler(dragRect_ER_mouseDown);
            //dragRect_ER.MouseMove += new System.Windows.Input.MouseEventHandler(dragRect_ER_mouseMove);
            //dragRect_ER.MouseLeftButtonUp += new MouseButtonEventHandler(dragRect_ER_mouseUp);

            edit_button_1_1.Click += new RoutedEventHandler(edit_button_1_Click);
            edit_button_2_1.Click += new RoutedEventHandler(edit_button_1_Click);
            edit_button_4_1.Click += new RoutedEventHandler(edit_button_1_Click);
            edit_button_6_1.Click += new RoutedEventHandler(edit_button_1_Click);
            edit_button_2_2.Click += new RoutedEventHandler(edit_button_2_Click);
            edit_button_4_2.Click += new RoutedEventHandler(edit_button_2_Click);
            edit_button_6_2.Click += new RoutedEventHandler(edit_button_2_Click);
            edit_button_4_3.Click += new RoutedEventHandler(edit_button_3_Click);
            edit_button_6_3.Click += new RoutedEventHandler(edit_button_3_Click);
            edit_button_4_4.Click += new RoutedEventHandler(edit_button_4_Click);
            edit_button_6_4.Click += new RoutedEventHandler(edit_button_4_Click);
            edit_button_6_5.Click += new RoutedEventHandler(edit_button_5_Click);
            edit_button_6_6.Click += new RoutedEventHandler(edit_button_6_Click);
            edit_button_ER.Click += new RoutedEventHandler(edit_button_1_Click);
            loadVideo_button_1_1.Click += new RoutedEventHandler(loadVideo_button_1_Click);
            loadVideo_button_2_1.Click += new RoutedEventHandler(loadVideo_button_1_Click);
            loadVideo_button_4_1.Click += new RoutedEventHandler(loadVideo_button_1_Click);
            loadVideo_button_6_1.Click += new RoutedEventHandler(loadVideo_button_1_Click);
            loadVideo_button_2_2.Click += new RoutedEventHandler(loadVideo_button_2_Click);
            loadVideo_button_4_2.Click += new RoutedEventHandler(loadVideo_button_2_Click);
            loadVideo_button_6_2.Click += new RoutedEventHandler(loadVideo_button_2_Click);
            loadVideo_button_4_3.Click += new RoutedEventHandler(loadVideo_button_3_Click);
            loadVideo_button_6_3.Click += new RoutedEventHandler(loadVideo_button_3_Click);
            loadVideo_button_4_4.Click += new RoutedEventHandler(loadVideo_button_4_Click);
            loadVideo_button_6_4.Click += new RoutedEventHandler(loadVideo_button_4_Click);
            loadVideo_button_6_5.Click += new RoutedEventHandler(loadVideo_button_5_Click);
            loadVideo_button_6_6.Click += new RoutedEventHandler(loadVideo_button_6_Click);
            loadImage_button_1_1.Click += new RoutedEventHandler(loadImage_button_1_Click);
            loadImage_button_2_1.Click += new RoutedEventHandler(loadImage_button_1_Click);
            loadImage_button_4_1.Click += new RoutedEventHandler(loadImage_button_1_Click);
            loadImage_button_6_1.Click += new RoutedEventHandler(loadImage_button_1_Click);
            loadImage_button_ER.Click += new RoutedEventHandler(loadImage_button_1_Click);
            loadImage_button_2_2.Click += new RoutedEventHandler(loadImage_button_2_Click);
            loadImage_button_4_2.Click += new RoutedEventHandler(loadImage_button_2_Click);
            loadImage_button_6_2.Click += new RoutedEventHandler(loadImage_button_2_Click);
            loadImage_button_4_3.Click += new RoutedEventHandler(loadImage_button_3_Click);
            loadImage_button_6_3.Click += new RoutedEventHandler(loadImage_button_3_Click);
            loadImage_button_4_4.Click += new RoutedEventHandler(loadImage_button_4_Click);
            loadImage_button_6_4.Click += new RoutedEventHandler(loadImage_button_4_Click);
            loadImage_button_6_5.Click += new RoutedEventHandler(loadImage_button_5_Click);
            loadImage_button_6_6.Click += new RoutedEventHandler(loadImage_button_6_Click);
            sound_button_1_1.Click += new RoutedEventHandler(sound_button_1_Click);
            sound_button_2_1.Click += new RoutedEventHandler(sound_button_1_Click);
            sound_button_4_1.Click += new RoutedEventHandler(sound_button_1_Click);
            sound_button_6_1.Click += new RoutedEventHandler(sound_button_1_Click);
            sound_button_2_2.Click += new RoutedEventHandler(sound_button_2_Click);
            sound_button_4_2.Click += new RoutedEventHandler(sound_button_2_Click);
            sound_button_6_2.Click += new RoutedEventHandler(sound_button_2_Click);
            sound_button_4_3.Click += new RoutedEventHandler(sound_button_3_Click);
            sound_button_6_3.Click += new RoutedEventHandler(sound_button_3_Click);
            sound_button_4_4.Click += new RoutedEventHandler(sound_button_4_Click);
            sound_button_6_4.Click += new RoutedEventHandler(sound_button_4_Click);
            sound_button_6_5.Click += new RoutedEventHandler(sound_button_5_Click);
            sound_button_6_6.Click += new RoutedEventHandler(sound_button_6_Click);
            delete1246_button_1_1.Click += new RoutedEventHandler(delete1246_button_1_Click);
            delete1246_button_2_1.Click += new RoutedEventHandler(delete1246_button_1_Click);
            delete1246_button_4_1.Click += new RoutedEventHandler(delete1246_button_1_Click);
            delete1246_button_6_1.Click += new RoutedEventHandler(delete1246_button_1_Click);
            delete1246_button_2_2.Click += new RoutedEventHandler(delete1246_button_2_Click);
            delete1246_button_4_2.Click += new RoutedEventHandler(delete1246_button_2_Click);
            delete1246_button_6_2.Click += new RoutedEventHandler(delete1246_button_2_Click);
            delete1246_button_4_3.Click += new RoutedEventHandler(delete1246_button_3_Click);
            delete1246_button_6_3.Click += new RoutedEventHandler(delete1246_button_3_Click);
            delete1246_button_4_4.Click += new RoutedEventHandler(delete1246_button_4_Click);
            delete1246_button_6_4.Click += new RoutedEventHandler(delete1246_button_4_Click);
            delete1246_button_6_5.Click += new RoutedEventHandler(delete1246_button_5_Click);
            delete1246_button_6_6.Click += new RoutedEventHandler(delete1246_button_6_Click);
        }



        #endregion

        #region Flow
        int number_of_buttons;

        [STAThread]
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            Renew_path();

            ShowQuestion();
            ShowEditorUI();

            EC = new EyeCanvas();
            EC.Show();
            homeWindow = new MainWindow();

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(ThreadTimer);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timer.IsEnabled = true;

            timer_stopwatch = new System.Windows.Threading.DispatcherTimer();
            timer_stopwatch.Tick += new EventHandler(time_tick);
            timer_stopwatch.Interval = new TimeSpan(0, 0, 0, 0, 100);

            soundPlayer_play = new MediaPlayer();
            soundPlayer_play.MediaEnded += new EventHandler(sound_play_MediaEnded);
            soundPlayer_edit = new MediaPlayer();
            soundPlayer_edit.MediaEnded += new EventHandler(sound_edit_MediaEnded);

        }
        private void ShowQuestion()
        {
            string[] directoryPaths = Directory.GetDirectories(path_subject);
            int number_of_Directories = directoryPaths.Length;

            if (number_of_Directories == 0)
            {
                Global.currentQuestion = 0;
                new_question_no.Visibility = Visibility.Collapsed;
                new_question_box.Visibility = Visibility.Visible;
            }
            else
            {
                Global.currentQuestion = 1;
                LoadQuestion();
            }
        }
        private void ShowEditorUI()
        {
            blocks.SetValue(Grid.RowSpanProperty, 1);
            blocks.SetValue(Grid.RowProperty, 1);
            blocks.SetValue(Grid.ColumnSpanProperty, 1);
            blocks.SetValue(Grid.ColumnProperty, 1);
            subjectLabel.Content = Global.currentSubject;
            edit_UI.Visibility = Visibility.Visible;
            Thickness marginBottom = topBar.Margin;
            marginBottom.Bottom = 0;
            topBar.Margin = marginBottom;
            Thickness marginTop = topBar.Margin;
            marginTop.Top = 0;
            bottomBar.Margin = marginTop;
            base_buttons.Opacity = 1;
            sideBar.Visibility = Visibility.Visible;
            this.chooseOff_button.Visibility = Visibility.Collapsed;

            fullScreen_button.Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Editor/buttons/fullScreen.png", UriKind.Relative)));
            if (Global.current1246 == ER)
                add_ER.Visibility = Visibility.Visible;


        }
        private void ShowFullScreenUI()
        {
            base_buttons.Opacity = 0;
            edit_UI.Visibility = Visibility.Hidden;
            blocks.SetValue(Grid.RowSpanProperty, 3);
            blocks.SetValue(Grid.RowProperty, 0);
            blocks.SetValue(Grid.ColumnSpanProperty, 3);
            blocks.SetValue(Grid.ColumnProperty, 0);
            Thickness marginBotton = topBar.Margin;
            marginBotton.Bottom = 150;
            topBar.Margin = marginBotton;
            Thickness marginTop = topBar.Margin;
            marginTop.Top = 150;
            bottomBar.Margin = marginTop;
            fullScreen_button.Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Editor/buttons/fullScreen_hover.png", UriKind.Relative)));
            sideBar.Visibility = Visibility.Collapsed;
            chooseOff_button.Visibility = Visibility.Visible;

            for (int i = 0; i < number_of_buttons; i++)
            {
                load_image_button_array[i].Visibility = Visibility.Collapsed;
                if (Global.current1246 != ER)
                {
                    edit_button_array[i].Visibility = Visibility.Collapsed;
                    load_video_button_array[i].Visibility = Visibility.Collapsed;
                    sound_button_array[i].Visibility = Visibility.Collapsed;
                    delete1246_button_array[i].Visibility = Visibility.Collapsed;
                }
            }

            if (Global.current1246 == ER)
            {
                inkCanvas_ER.Children.Clear();
                add_ER.Visibility = Visibility.Collapsed;
            }


        }
        #endregion

        #region ButtonClick
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //if (e.Key == Key.Escape)
            //{
            //    homeWindow.Show();
            //    this.Close();
            //}

            //if (e.Key == Key.Left)
            //{
            //    PreQuestion();
            //}
            //if (e.Key == Key.Right)
            //{
            //    NextQuestion();
            //}
            if (e.Key == Key.Insert)
            {
                Point pointToWindow = Mouse.GetPosition(this);
                Point pointToScreen = PointToScreen(pointToWindow);
                mouse_event(MOUSEEVENTF_LEFTDOWN, (int)pointToScreen.X, (int)pointToScreen.Y, 0, 0);
                Thread.Sleep(100);
                mouse_event(MOUSEEVENTF_LEFTUP, (int)pointToScreen.X, (int)pointToScreen.Y, 0, 0);
            }
        }

        private void fullScreen_button_Click(object sender, RoutedEventArgs e)
        {
            if (mode == "editor")
            {
                mode = "fullscreen";
                chooseMode = false;
                ShowFullScreenUI();
                StartFullScreenHandler();
                ResizeRect();
            }
            else if (mode == "fullscreen")
            {
                mode = "editor";
                ShowEditorUI();
                LoadQuestion();
                CloseFullScreenHandler();
                ResizeRect();
                EC.StopDrawDot();
            }
        }
        
        private void Add_Question_Click(object sender, RoutedEventArgs e)
        {
            if (Global.currentQuestion == 0)
            {
                new_question_no.Visibility = Visibility.Collapsed;
            }
            else
            {
                new_question_no.Visibility = Visibility.Visible;
            }
            new_question_box.Visibility = Visibility.Visible;
        }
        private void Delete_Question_Click(object sender, RoutedEventArgs e)
        {
            delete_box.Visibility = Visibility.Visible;
        }
        private void delete_question_Yes_Click(object sender, RoutedEventArgs e)
        {
            string[] directoryPaths_subject = Directory.GetDirectories(path_subject);
            for (int i = 0; i < number_of_buttons; i++)
            {
                if (Global.current1246 != ER)
                    video_array[i].Source = null;
                image_array[i].Source = null;
            }
            DeleteDirectory(path_question);
            Thread.Sleep(300);

            if (directoryPaths_subject.Length == 1)
            {
                Global.current1246 = 0;
                Global.currentQuestion = Global.currentQuestion - 1;
                ShowQuestion();
            }
            else
            {
                if (Global.currentQuestion == directoryPaths_subject.Length)
                {
                    Global.currentQuestion = Global.currentQuestion - 1;
                }
                else
                {
                    RenameQuestion_Delete(directoryPaths_subject.Length);
                }
                LoadQuestion();
            }
            delete_box.Visibility = Visibility.Collapsed;
        }

        private void delete_question_No_Click(object sender, RoutedEventArgs e)
        {
            delete_box.Visibility = Visibility.Collapsed;
        }
        private void previous_Click(object sender, RoutedEventArgs e)
        {
            if (Global.current1246 == ER)
            {
                SaveSmallRect();
            }
            PreQuestion();
        }
        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (Global.current1246 == ER)
            {
                SaveSmallRect();
            }
            NextQuestion();
        }

        private void new_question_1_Click(object sender, RoutedEventArgs e)
        {
            Global.current1246 = 1;

            RenameQuestion_New();
            Global.currentQuestion += 1;
            Renew_path();
            Directory.CreateDirectory(path_question);
            using (File.Create(path_question + @"/1.txt")) { }
            //File.CreateText(path_question + @"/1.txt");
            new_question_box.Visibility = Visibility.Collapsed;
            LoadQuestion();
        }
        private void new_question_2_Click(object sender, RoutedEventArgs e)
        {
            Global.current1246 = 2;

            RenameQuestion_New();
            Global.currentQuestion += 1;
            Renew_path();
            Directory.CreateDirectory(path_question);
            using (File.Create(path_question + @"/2.txt")) { }
            new_question_box.Visibility = Visibility.Collapsed;
            LoadQuestion();
        }
        private void new_question_4_Click(object sender, RoutedEventArgs e)
        {
            Global.current1246 = 4;

            RenameQuestion_New();
            Global.currentQuestion += 1;
            Renew_path();
            Directory.CreateDirectory(path_question);
            using (File.Create(path_question + @"/4.txt")) { }
            new_question_box.Visibility = Visibility.Collapsed;
            LoadQuestion();
        }
        private void new_question_6_Click(object sender, RoutedEventArgs e)
        {
            Global.current1246 = 6;

            RenameQuestion_New();
            Global.currentQuestion += 1;
            Renew_path();
            Directory.CreateDirectory(path_question);
            using (File.Create(path_question + @"/6.txt")) { }
            new_question_box.Visibility = Visibility.Collapsed;
            LoadQuestion();
        }
        private void new_question_ER_Click(object sender, RoutedEventArgs e)
        {
            Global.current1246 = ER;

            RenameQuestion_New();
            Global.currentQuestion += 1;
            Renew_path();
            Directory.CreateDirectory(path_question);
            using (File.Create(path_question + @"/ER.txt")) { }
            using (StreamWriter writer = new StreamWriter(path_question + @"/ER_text.txt"))
            {
                for (int i = 0; i < 10; i++)
                {
                    writer.WriteLine(i + ",");
                }
            }
            new_question_box.Visibility = Visibility.Collapsed;
            LoadQuestion();
        }
        private void new_question_no_Click(object sender, RoutedEventArgs e)
        {
            new_question_box.Visibility = Visibility.Collapsed;
        }

        private void choose_sound_button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openAudioFile = new OpenFileDialog();
            //openAudioFile.InitialDirectory = (Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            openAudioFile.Filter = "Audio Files|*.ASX;*.B4S;*.M3U;*.PLS;*.WPL;*.ZPL;*.AAC;*.AC3; *.APE; *.CDA; *.DTS; *.FLAC; *.MID; *.MP3; *.MPA; *.MPC; *.OGG; *.RA; *.WAV; *.WMA; ";
            DialogResult openAudioResult = openAudioFile.ShowDialog();
            if (openAudioResult == System.Windows.Forms.DialogResult.OK)
            {
                if (IMG_Sound)
                {
                    string[] filePaths = Directory.GetFiles(path_question);
                    string sound_file_path = path_question + @"/sound" + sound1246Num + System.IO.Path.GetExtension(filePaths[Sound_Number]);
                    if (File.Exists(sound_file_path)) File.Delete(sound_file_path);
                }
                string AudioPath = openAudioFile.FileName;
                string currentSoundPath = string.Format(path_question + @"/sound" + sound1246Num + System.IO.Path.GetExtension(AudioPath));
                File.Copy(AudioPath, currentSoundPath);
                LoadQuestion();
            }
            else
            {
                LoadQuestion();
            }
            if (Global.current1246 == ER)
            {
                soundBox_ER.Visibility = Visibility.Collapsed;
            }
            else
            {
                choose_sound_box_array[sound1246Num - 1].Visibility = Visibility.Collapsed;
            }
        }
        private void record_sound_button_Click(object sender, RoutedEventArgs e)
        {
            if (Global.current1246 == ER)
            {
                soundBox_ER.Visibility = Visibility.Collapsed;
            }
            else
            {
                choose_sound_box_array[sound1246Num - 1].Visibility = Visibility.Collapsed;
            }
            record_box.Visibility = Visibility.Visible;

            if (!IMG_Sound)
            {
                play_button.Visibility = Visibility.Collapsed;
                finish_button.Visibility = Visibility.Collapsed;
                record_stopwatch.Visibility = Visibility.Collapsed;
            }
        }
        private void TTS_sound_button_Click(object sender, RoutedEventArgs e)
        {
            TTS_box.Visibility = Visibility.Visible;
        }
        private void TTS_Yes_Click(object sender, RoutedEventArgs e)
        {
            String input = TTStextBox.Text;
            if (string.IsNullOrWhiteSpace(input))
            {
                message_box2.Visibility = Visibility.Visible;
            }
            else
            {
                TTS_box.Visibility = Visibility.Collapsed;
                if (IMG_Sound)
                {
                    string[] filePaths = Directory.GetFiles(path_question);
                    string sound_file_path = path_question + @"/sound" + sound1246Num + System.IO.Path.GetExtension(filePaths[Sound_Number]);
                    if (File.Exists(sound_file_path)) File.Delete(sound_file_path);
                }
                String TTS_txt_path = String.Format(path_question + @"/sound" + sound1246Num +".txt");
                using (StreamWriter writer = new StreamWriter(TTS_txt_path))
                {
                    writer.WriteLine(input);
                }
                TTStextBox.Text = String.Empty;
                if (Global.current1246 == ER)
                {
                    soundBox_ER.Visibility = Visibility.Collapsed;
                }
                else
                {
                    choose_sound_box_array[sound1246Num - 1].Visibility = Visibility.Collapsed;
                }
                LoadQuestion();
            }

        }
        private void TTS_No_Click(object sender, RoutedEventArgs e)
        {
            TTS_box.Visibility = Visibility.Collapsed;
            if (Global.current1246 == ER)
            {
                soundBox_ER.Visibility = Visibility.Collapsed;
            }
            else
            {
                choose_sound_box_array[sound1246Num - 1].Visibility = Visibility.Collapsed;
            }
        }
        private void message_box2_Yes_Click(object sender, RoutedEventArgs e)
        {
            message_box2.Visibility = Visibility.Collapsed;
        }

        private void choose_sound_no_Click(object sender, RoutedEventArgs e)
        {
            if (Global.current1246 == ER)
            {
                soundBox_ER.Visibility = Visibility.Collapsed;
            }
            else
            {
                choose_sound_box_array[sound1246Num - 1].Visibility = Visibility.Collapsed;
            }
        }

        private void record_button_Click(object sender, RoutedEventArgs e)
        {
            play_button.Visibility = Visibility.Visible;
            finish_button.Visibility = Visibility.Visible;
            play_button.Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Editor/recordBox/recordPlayBtn.png", UriKind.Relative)));
            play_button.IsEnabled = false;
            finish_button.Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Editor/recordBox/recordFinishBtn.png", UriKind.Relative)));
            finish_button.IsEnabled = false;
            record_button.Visibility = Visibility.Collapsed;
            record_stop_button.Visibility = Visibility.Visible;
            close_record_box_button.IsEnabled = false;
            record_stopwatch.Visibility = Visibility.Visible;

            string[] filePaths = Directory.GetFiles(path_question);
            if (IMG_Sound)
            {
                string sound_file_path = path_question + @"/sound" + sound1246Num + System.IO.Path.GetExtension(filePaths[Sound_Number]);
                if (File.Exists(sound_file_path)) File.Delete(sound_file_path);
            }

            waveSource = new WaveIn();
            waveSource.WaveFormat = new WaveFormat(44100, 1);

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            waveFile = new WaveFileWriter(path_question + @"/sound" + sound1246Num + @".wav", waveSource.WaveFormat);

            waveSource.StartRecording();

            stopwatch = new Stopwatch();
            stopwatch.Start();
            timer_stopwatch.IsEnabled = true;

        }
        private void record_stop_button_Click(object sender, RoutedEventArgs e)
        {
            record_stop_button.Visibility = Visibility.Collapsed;
            record_button.Visibility = Visibility.Visible;
            play_button.Visibility = Visibility.Visible;
            play_button.Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Editor/recordBox/recordPlayBtn_active.png", UriKind.Relative)));
            play_button.IsEnabled = true;
            finish_button.Visibility = Visibility.Visible;
            finish_button.Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Editor/recordBox/recordFinishBtn_active.png", UriKind.Relative)));
            finish_button.IsEnabled = true;
            close_record_box_button.IsEnabled = true;
            waveSource.StopRecording();
            stopwatch.Stop();
            timer_stopwatch.IsEnabled = false;
        }
        private void play_button_Click(object sender, RoutedEventArgs e)
        {
            string sound_file_path = path_question + @"/sound" + sound1246Num + @".wav";
            if (File.Exists(sound_file_path))
            {
                play_button.IsEnabled = false;
                finish_button.IsEnabled = false;
                record_button.IsEnabled = false;
                close_record_box_button.IsEnabled = false;
                soundPlayer_edit.Open(new Uri(sound_file_path));
                soundPlayer_edit.Play();
                stopwatch = new Stopwatch();
                stopwatch.Start();
                timer_stopwatch.IsEnabled = true;
            }
        }
        private void close_record_box_button_Click(object sender, RoutedEventArgs e)
        {
            string sound_file_path = path_question + @"/sound" + sound1246Num + @".wav";
            if (File.Exists(sound_file_path)) File.Delete(path_question + @"/sound" + sound1246Num + @".wav");
            record_box.Visibility = Visibility.Collapsed;
        }
        private void finish_button_Click(object sender, RoutedEventArgs e)
        {
            record_box.Visibility = Visibility.Collapsed;
            //if(Global.current1246 == ER)
            //{
            //    sound_button_ER.Visibility
            //}
            //sound_button_array[sound1246Num - 1].Background = new ImageBrush(new BitmapImage(new Uri(@"Images/sound.png", UriKind.Relative)));
            LoadQuestion();
        }

        private void add_ER_Click(object sender, RoutedEventArgs e)
        {
            Rectangle newShape = new Rectangle();
            InkCanvas.SetLeft(newShape, 100);
            InkCanvas.SetTop(newShape, 100);
            newShape.Width = 300;
            newShape.Height = 300;
            newShape.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 203, 139));
            newShape.StrokeThickness = 5;
            newShape.Fill = new SolidColorBrush(Color.FromArgb(50, 255, 203, 139));
            DoubleCollection aa = new DoubleCollection() { 4, 2 };
            newShape.StrokeDashArray = aa;

            newShape.MouseLeftButtonDown += new MouseButtonEventHandler(dragRect_ER_mouseDown);
            newShape.MouseMove += new System.Windows.Input.MouseEventHandler(dragRect_ER_mouseMove);
            newShape.MouseLeftButtonUp += new MouseButtonEventHandler(dragRect_ER_mouseUp);

            rectangles.Add(newShape);

            show_rectangles();
        }
        private void delete_button_ER_Click(object sender, RoutedEventArgs e)
        {
            rectangles.RemoveAt(select_rect_index);
            inkCanvas_ER.Children.RemoveAt(select_rect_index);
            delete_button_ER.Visibility = Visibility.Collapsed;
            sound_button_ER.Visibility = Visibility.Collapsed;
            text_ER.Visibility = Visibility.Collapsed;
            soundBox_ER.Visibility = Visibility.Collapsed;
            ER_text = "";

            string[] filePaths = Directory.GetFiles(path_question);
            if (IMG_Sound)
            {
                string path_sound_select = path_question + @"/sound" + select_rect_index + System.IO.Path.GetExtension(filePaths[Sound_Number]);
                if (File.Exists(path_sound_select))
                    File.Delete(path_sound_select);

                for (int i = select_rect_index + 1; i < 20; i++)
                {
                    string path_sound = path_question + @"/sound" + i + System.IO.Path.GetExtension(filePaths[Sound_Number]);
                    if (File.Exists(path_sound))
                    {
                        Directory.Move(path_sound, String.Format(path_question + @"/sound" + "{0}" + System.IO.Path.GetExtension(filePaths[Sound_Number]), i - 1));
                    }
                }
            }
        }
        private void sound_button_ER_Click(object sender, RoutedEventArgs e)
        {
            if(soundBox_ER.Visibility == Visibility.Collapsed)
                soundBox_ER.Visibility = Visibility.Visible;
            else
                soundBox_ER.Visibility = Visibility.Collapsed;
            sound1246Num = select_rect_index;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            homeWindow.Show();
            this.Close();
        }
        private void minimize_button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void close_button_Click(object sender, RoutedEventArgs e)
        {
            close_app.Visibility = Visibility.Visible;
        }
        private void close_app_Yes_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            try
            {
                System.Windows.Application.Current.Shutdown();
                System.Environment.Exit(System.Environment.ExitCode);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("catch close error : " + ex.ToString());
            }
        }
        bool chooseMode = false;
        private void chooseOff_button_Click(object sender, RoutedEventArgs e)
        {
            if (this.chooseMode)
            {
                this.chooseMode = false;
                this.chooseOff_button.Background = (Brush)new ImageBrush((ImageSource)new BitmapImage(new Uri("Images/Editor/buttons/chooseOff.png", UriKind.Relative)));
            }
            else
            {
                this.chooseMode = true;
                this.chooseOff_button.Background = (Brush)new ImageBrush((ImageSource)new BitmapImage(new Uri("Images/Editor/buttons/chooseOn.png", UriKind.Relative)));
            }
        }
        private void close_app_No_Click(object sender, RoutedEventArgs e)
        {
            close_app.Visibility = Visibility.Collapsed;
        }
        private void edit_button_1_Click(object sender, RoutedEventArgs e)
        {
            EditStart(1);
        }
        private void edit_button_2_Click(object sender, RoutedEventArgs e)
        {
            EditStart(2);
        }
        private void edit_button_3_Click(object sender, RoutedEventArgs e)
        {
            EditStart(3);
        }
        private void edit_button_4_Click(object sender, RoutedEventArgs e)
        {
            EditStart(4);
        }
        private void edit_button_5_Click(object sender, RoutedEventArgs e)
        {
            EditStart(5);
        }
        private void edit_button_6_Click(object sender, RoutedEventArgs e)
        {
            EditStart(6);
        }
        private void loadVideo_button_1_Click(object sender, RoutedEventArgs e)
        {
            loadVideo(1);
        }
        private void loadVideo_button_2_Click(object sender, RoutedEventArgs e)
        {
            loadVideo(2);
        }
        private void loadVideo_button_3_Click(object sender, RoutedEventArgs e)
        {
            loadVideo(3);
        }
        private void loadVideo_button_4_Click(object sender, RoutedEventArgs e)
        {
            loadVideo(4);
        }
        private void loadVideo_button_5_Click(object sender, RoutedEventArgs e)
        {
            loadVideo(5);
        }
        private void loadVideo_button_6_Click(object sender, RoutedEventArgs e)
        {
            loadVideo(6);
        }
        private void loadImage_button_1_Click(object sender, RoutedEventArgs e)
        {
            loadImage(1);
        }
        private void loadImage_button_2_Click(object sender, RoutedEventArgs e)
        {
            loadImage(2);
        }
        private void loadImage_button_3_Click(object sender, RoutedEventArgs e)
        {
            loadImage(3);
        }
        private void loadImage_button_4_Click(object sender, RoutedEventArgs e)
        {
            loadImage(4);
        }
        private void loadImage_button_5_Click(object sender, RoutedEventArgs e)
        {
            loadImage(5);
        }
        private void loadImage_button_6_Click(object sender, RoutedEventArgs e)
        {
            loadImage(6);
        }
        private void sound_button_1_Click(object sender, RoutedEventArgs e)
        {
            loadSound(1);
        }
        private void sound_button_2_Click(object sender, RoutedEventArgs e)
        {
            loadSound(2);
        }
        private void sound_button_3_Click(object sender, RoutedEventArgs e)
        {
            loadSound(3);
        }
        private void sound_button_4_Click(object sender, RoutedEventArgs e)
        {
            loadSound(4);
        }
        private void sound_button_5_Click(object sender, RoutedEventArgs e)
        {
            loadSound(5);
        }
        private void sound_button_6_Click(object sender, RoutedEventArgs e)
        {
            loadSound(6);
        }
        private void delete1246_button_1_Click(object sender, RoutedEventArgs e)
        {
            delete1246(1);
        }
        private void delete1246_button_2_Click(object sender, RoutedEventArgs e)
        {
            delete1246(2);
        }
        private void delete1246_button_3_Click(object sender, RoutedEventArgs e)
        {
            delete1246(3);
        }
        private void delete1246_button_4_Click(object sender, RoutedEventArgs e)
        {
            delete1246(4);
        }
        private void delete1246_button_5_Click(object sender, RoutedEventArgs e)
        {
            delete1246(5);
        }
        private void delete1246_button_6_Click(object sender, RoutedEventArgs e)
        {
            delete1246(6);
        }

        #endregion

        #region EventListener
        void sound_play_MediaEnded(object sender, EventArgs e)
        {
            soundPlayer_play.Close();
            SoundCompleted();
        }
        void SayCompleted(object sender, SpeakCompletedEventArgs e)
        {
            isSpeech = false;
            reader.Dispose();
            SoundCompleted();
        }
        private void SoundCompleted()
        {
            //**hover_style**
            //string[] filePaths = Directory.GetFiles(path_question);
            //string path_image = string.Format(path_question + @"/image" + selectIndex + System.IO.Path.GetExtension(filePaths[Image_Number]));
            //if (File.Exists(path_image)) image_array[selectIndex - 1].Source = BitmapFromUri(path_image);
            if (Global.current1246 != ER)
                hover_border_array[selectIndex - 1].Visibility = Visibility.Collapsed;

            EC.ellipse.Width = Global.ellipseSize;
            EC.ellipse.Height = Global.ellipseSize;
            oldIndex = -1;
            selectTimeStore = false;
            timer.IsEnabled = true;
            previous.IsEnabled = true;
            next.IsEnabled = true;
            if (chooseMode)
            {
                if (Global.current1246 == this.ER)
                    SaveSmallRect();
                NextQuestion();
            }
            inkCanvas_ER.Children.Clear();
            StartFullScreenHandler();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (video_array != null)
            {
                for (int i = 0; i < number_of_buttons; i++)
                {
                    if (Global.current1246 != ER)
                        video_array[i].Stop();
                }
            }
            EC.Close();

            if (soundPlayer_play != null) soundPlayer_play.Stop();
            if (timer != null)
            {
                timer.Stop();
                timer.IsEnabled = false;
            }
        }
        void sound_edit_MediaEnded(object sender, EventArgs e)
        {
            soundPlayer_edit.Close();
            play_button.IsEnabled = true;
            finish_button.IsEnabled = true;
            record_button.IsEnabled = true;
            close_record_box_button.IsEnabled = true;
            stopwatch.Stop();
            timer_stopwatch.IsEnabled = false;
        }

        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();
            }
        }
        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }
        }
        bool mousePress_dragRect_ER;
        Point mouseDown_Point;
        private void dragRect_ER_mouseDown(object sender, EventArgs e)
        {
            if(mode == "fullscreen") { return; }
            mousePress_dragRect_ER = true;

            mouseDown_Point = new Point(InkCanvas.GetLeft(rectangles[select_rect_index]),InkCanvas.GetTop(rectangles[select_rect_index]));
        }
        private void dragRect_ER_mouseMove(object sender, EventArgs e)
        {
            if (mousePress_dragRect_ER)
            {
                Console.WriteLine("moving");
                Point pointToWindow = Mouse.GetPosition(this);
                Point p_mouse = PointToScreen(pointToWindow);
                Point p_inkCanvas = inkCanvas_ER.PointToScreen(new Point(0, 0));
                Point p_rect = rectangles[select_rect_index].PointToScreen(new Point(0, 0));
                double screen_ratio = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;

                InkCanvas.SetTop(rectangles[select_rect_index], p_mouse.Y / screen_ratio - p_inkCanvas.Y);
                InkCanvas.SetLeft(rectangles[select_rect_index], p_mouse.X / screen_ratio - p_inkCanvas.X);
                set_ER_buttons();
            }
        }
        private void dragRect_ER_mouseUp(object sender, EventArgs e)
        {
            mousePress_dragRect_ER = false;

            for (int i = 0; i < rectangles.Count; i++)
            {
                if (i != select_rect_index)
                {
                    if (isOverlap(rectangles[select_rect_index], rectangles[i]))
                    {
                        Point pp = inkCanvas_ER.PointToScreen(new Point(0, 0));
                        double screen_ratio = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;

                        InkCanvas.SetTop(rectangles[select_rect_index], mouseDown_Point.Y );
                        InkCanvas.SetLeft(rectangles[select_rect_index], mouseDown_Point.X);
                        set_ER_buttons();
                    }
                    else
                    {
                        save_ER();
                    }
                }
            }
        }
        private void inkCanvas_ER_SelectionMoved(object sender, EventArgs e)
        {
            Console.WriteLine("moved");
            set_ER_buttons();
            save_ER();
        }
        private void inkCanvas_ER_SelectionResized(object sender, EventArgs e)
        {
            Console.WriteLine("resized");
            set_ER_buttons();
            save_ER();
        }
        private void inkCanvas_ER_SelectionChanged(object sender, EventArgs e)
        {
            Console.WriteLine("changed");
            set_ER_buttons();
            save_ER();

            load_ER_text();
        }
        private void inkCanvas_ER_SelectionChanging(object sender, InkCanvasSelectionChangingEventArgs e)
        {
            System.Collections.ObjectModel.ReadOnlyCollection<UIElement> elements = e.GetSelectedElements();
            select_rect_index = -1;
            for (int i = 0; i < rectangles.Count; i++)
            {
                if (elements.Contains(rectangles[i]))
                {
                    select_rect_index = i;
                }
            }
            Console.WriteLine(select_rect_index);
        }
        private void inkCanvas_ER_SelectionMoving(object sender, InkCanvasSelectionEditingEventArgs e)
        {
            for (int i = 0; i < rectangles.Count; i++)
            {
                if (i != select_rect_index)
                {
                    Rectangle newRectangle = new Rectangle();
                    InkCanvas.SetLeft(newRectangle, e.NewRectangle.Left);
                    InkCanvas.SetTop(newRectangle, e.NewRectangle.Top);
                    newRectangle.Width = e.NewRectangle.Width;
                    newRectangle.Height = e.NewRectangle.Height;
                    if (isOverlap(newRectangle, rectangles[i]))
                    {
                        inkCanvas_ER.Children.Clear();
                        foreach (var a in rectangles)
                        {
                            inkCanvas_ER.Children.Add(a);
                        }
                        inkCanvas_ER.Children.Add(sound_button_ER);
                        inkCanvas_ER.Children.Add(delete_button_ER);
                        inkCanvas_ER.Children.Add(soundBox_ER);
                        inkCanvas_ER.Children.Add(text_ER);
                    }
                }
            }
        }
        private void inkCanvas_ER_SelectionResizing(object sender, InkCanvasSelectionEditingEventArgs e)
        {
            Rectangle newRectangle = new Rectangle();
            InkCanvas.SetLeft(newRectangle, e.NewRectangle.Left);
            InkCanvas.SetTop(newRectangle, e.NewRectangle.Top);
            newRectangle.Width = e.NewRectangle.Width;
            newRectangle.Height = e.NewRectangle.Height;
            for (int i = 0; i < rectangles.Count; i++)
            {
                if (i != select_rect_index)
                {
                    if (isOverlap(newRectangle, rectangles[i]))
                    {
                        inkCanvas_ER.Children.Clear();
                        foreach (var a in rectangles)
                        {
                            inkCanvas_ER.Children.Add(a);
                        }
                        inkCanvas_ER.Children.Add(sound_button_ER);
                        inkCanvas_ER.Children.Add(delete_button_ER);
                        inkCanvas_ER.Children.Add(soundBox_ER);
                        inkCanvas_ER.Children.Add(text_ER);
                        
                    }
                }
            }
            if (isTooSmall(newRectangle))
            {
                inkCanvas_ER.Children.Clear();
                foreach (var a in rectangles)
                {
                    inkCanvas_ER.Children.Add(a);
                }
                inkCanvas_ER.Children.Add(sound_button_ER);
                inkCanvas_ER.Children.Add(delete_button_ER);
                inkCanvas_ER.Children.Add(soundBox_ER);
                inkCanvas_ER.Children.Add(text_ER);
                
            }
        }
        #endregion

        #region EyeControl

        List<Point> eyePointsList = new List<Point>();

        private int selectIndex = -1;
        private int oldIndex = -1;
        DateTime selectTime;
        Boolean selectTimeStore = false;

        int Video_Number;
        int Image_Number;
        int Sound_Number;
        Boolean IMG_Sound;

        public string IMG_VID(int focusSection)
        {
            string retStr = "nothing";
            IMG_Sound = false;
            string path = String.Format(path_question);
            string[] filePaths = Directory.GetFiles(path);
            for (int i = 0; i < filePaths.Length; i++)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(filePaths[i]);
                char[] charFileName = fileName.ToCharArray();
                if (charFileName.Length == 6)
                {
                    if (charFileName[5].ToString() == focusSection.ToString())
                    {
                        string fileType = new string(charFileName, 0, 5);
                        if (fileType == "video")
                        {
                            retStr = "video";
                            Video_Number = i;
                        }
                        else if (fileType == "image")
                        {
                            retStr = "image";
                            Image_Number = i;
                        }
                        else if (fileType == "sound")
                        {
                            IMG_Sound = true;
                            Sound_Number = i;
                        }
                    }
                }

            }
            return retStr;
        }
        public void ThreadTimer(object sender, EventArgs e)
        {
            if (mode == "fullscreen")
            {
                EC.StartDrawDot();
                eyeTrackBlock();

                if (Global.current1246 != ER)
                {
                    if (IMG_VID(selectIndex) == "video")
                    {
                        eyeFocusVideo();
                    }
                    else if (IMG_VID(selectIndex) == "image")
                    {
                        for (int i = 0; i < number_of_buttons; i++)
                        {
                            video_array[i].Pause();
                        }
                        eyeFocusImage();
                    }
                    else if (IMG_VID(selectIndex) == "nothing")
                    {
                        for (int i = 0; i < number_of_buttons; i++)
                        {
                            if (Global.current1246 != ER)
                                video_array[i].Pause();
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("ThreadTimer Bug");
                    }
                }
                else
                {
                    IMG_VID(selectIndex);
                    if (selectIndex != -1)
                        eyeFocus_ER();
                }
            }
            else
            {
                EC.StopDrawDot();
                if (Global.current1246 != ER)
                {
                    oldIndex = -1;
                    if (video_array != null)
                    {
                        for (int i = 0; i < number_of_buttons; i++)
                        {
                            video_array[i].Pause();
                        }
                    }
                }
            }
        }
        private void eyeTrackBlock()
        {
            if (EC != null)
            {
                Point eyePos = EC.GetEyePoint();
                Boolean isGaze = EC.GetGazeTracked();

                eyePointsList.Add(eyePos);
                if (isGaze == true)
                {
                    if (Global.current1246 == 1)
                    {
                        selectIndex = 1;
                    }
                    else if (Global.current1246 == 2)
                    {
                        if (eyePos.X < screenWidth * 0.5 && eyePos.X > screenWidth * 0)
                        {
                            selectIndex = 1;
                        }
                        else if (eyePos.X < screenWidth * 1 && eyePos.X > screenWidth * 0.5)
                        {
                            selectIndex = 2;
                        }
                    }
                    else if (Global.current1246 == 4)
                    {
                        if (eyePos.X < screenWidth * 0.5 && eyePos.X > screenWidth * 0 && eyePos.Y < screenHeight * 0.5 && eyePos.Y > screenHeight * 0)
                        {
                            selectIndex = 1;
                        }
                        else if (eyePos.X < screenWidth * 1 && eyePos.X > screenWidth * 0.5 && eyePos.Y < screenHeight * 0.5 && eyePos.Y > screenHeight * 0)
                        {
                            selectIndex = 2;
                        }
                        else if (eyePos.X < screenWidth * 0.5 && eyePos.X > screenWidth * 0 && eyePos.Y < screenHeight * 1 && eyePos.Y > screenHeight * 0.5)
                        {
                            selectIndex = 3;
                        }
                        else if (eyePos.X < screenWidth * 1 && eyePos.X > screenWidth * 0.5 && eyePos.Y < screenHeight * 1 && eyePos.Y > screenHeight * 0.5)
                        {
                            selectIndex = 4;
                        }
                    }
                    else if (Global.current1246 == 6)
                    {
                        if (eyePos.X < screenWidth * 0.33 && eyePos.X > screenWidth * 0 && eyePos.Y < screenHeight * 0.5 && eyePos.Y > screenHeight * 0)
                        {
                            selectIndex = 1;
                        }
                        else if (eyePos.X < screenWidth * 0.66 && eyePos.X > screenWidth * 0.33 && eyePos.Y < screenHeight * 0.5 && eyePos.Y > screenHeight * 0)
                        {
                            selectIndex = 2;
                        }
                        else if (eyePos.X < screenWidth * 1 && eyePos.X > screenWidth * 0.66 && eyePos.Y < screenHeight * 0.5 && eyePos.Y > screenHeight * 0)
                        {
                            selectIndex = 3;
                        }
                        else if (eyePos.X < screenWidth * 0.33 && eyePos.X > screenWidth * 0 && eyePos.Y < screenHeight * 1 && eyePos.Y > screenHeight * 0.5)
                        {
                            selectIndex = 4;
                        }
                        else if (eyePos.X < screenWidth * 0.66 && eyePos.X > screenWidth * 0.33 && eyePos.Y < screenHeight * 1 && eyePos.Y > screenHeight * 0.5)
                        {
                            selectIndex = 5;
                        }
                        else if (eyePos.X < screenWidth * 1 && eyePos.X > screenWidth * 0.66 && eyePos.Y < screenHeight * 1 && eyePos.Y > screenHeight * 0.5)
                        {
                            selectIndex = 6;
                        }
                    }
                    else if (Global.current1246 == ER)
                    {
                        selectIndex = -1;
                        for (int i = 0; i < rectangles.Count; i++)
                        {
                            double screen_ratio = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;
                            double X1 = InkCanvas.GetLeft(rectangles[i]) * screen_ratio;
                            double X2 = (InkCanvas.GetLeft(rectangles[i]) + rectangles[i].Width) * screen_ratio;
                            double Y1 = InkCanvas.GetTop(rectangles[i]) * screen_ratio;
                            double Y2 = (InkCanvas.GetTop(rectangles[i]) + rectangles[i].Height) * screen_ratio;
                            if (eyePos.X > X1 && eyePos.X < X2 && eyePos.Y > Y1 && eyePos.Y < Y2)
                            {
                                selectIndex = i;
                            }
                            //Console.WriteLine(X1 + ","+X2);

                        }
                        //Console.WriteLine(eyePos.X + "," + eyePos.Y);
                    }
                }
                else
                {
                    selectIndex = -1;
                }
            }
        }
        private void eyeFocusVideo()
        {
            for (int i = 0; i < number_of_buttons; i++)
            {
                video_array[i].Pause();
            }
            video_array[selectIndex - 1].Play();
        }
        public void eyeFocusImage()
        {
            if (selectIndex == oldIndex)
            {
                EC.ellipse.Width -= Global.ellipseSize / 50;
                EC.ellipse.Height -= Global.ellipseSize / 50;
            }
            else
            {
                EC.ellipse.Width = Global.ellipseSize;
                EC.ellipse.Height = Global.ellipseSize;
                oldIndex = selectIndex;
                selectTimeStore = false;
            }

            if (selectTimeStore == false)
            {
                selectTime = DateTime.Now;
                selectTimeStore = true;
            }
            
            if ((DateTime.Now - selectTime).TotalMilliseconds > gazeTime)
            {
                if (IMG_Sound == true)
                {
                    string[] filePaths = Directory.GetFiles(path_question);
                    string extendsion_type = System.IO.Path.GetExtension(filePaths[Sound_Number]);
                    if (extendsion_type == ".txt")
                    {
                        string strTTS = "";
                        String TTS_txt_path = String.Format(path_question + @"/sound" + selectIndex + ".txt");
                        if (File.Exists(TTS_txt_path))
                        {
                            using (StreamReader reader = new StreamReader(TTS_txt_path))
                            {
                                strTTS = reader.ReadLine();
                            }
                        }
                        Say(strTTS);
                    }
                    else
                    {
                        string path_sound = string.Format(path_question + @"/sound" + selectIndex + System.IO.Path.GetExtension(filePaths[Sound_Number]));
                        soundPlayer_play.Open(new Uri(path_sound));
                        soundPlayer_play.Play();
                    }
                }
                else
                {
                    soundPlayer_play.Open(new Uri(@"Sounds/System/silence.mp3", UriKind.Relative));
                    soundPlayer_play.Play();
                }
                //SetGrayscale(image_array[selectIndex - 1]);
                hover_border_array[selectIndex - 1].Visibility = Visibility.Visible;
                timer.IsEnabled = false;
                previous.IsEnabled = false;
                next.IsEnabled = false;
                CloseFullScreenHandler();
            }
        }
        SpeechSynthesizer reader;
        public bool isSpeech = false;
        public void Say(string str)
        {
            if (str == null || str == "") return;
            isSpeech = true;
            reader = new SpeechSynthesizer();
            reader.Rate = -3;
            reader.Volume = 100;
            reader.SpeakAsync(str);
            reader.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(SayCompleted);

        }
        
        private void eyeFocus_ER()
        {

            if (selectIndex == oldIndex)
            {
                EC.ellipse.Width -= Global.ellipseSize / 50;
                EC.ellipse.Height -= Global.ellipseSize / 50;
            }
            else
            {
                EC.ellipse.Width = Global.ellipseSize;
                EC.ellipse.Height = Global.ellipseSize;
                oldIndex = selectIndex;
                selectTimeStore = false;
            }

            if (selectTimeStore == false)
            {
                selectTime = DateTime.Now;
                selectTimeStore = true;
            }

            if ((DateTime.Now - selectTime).TotalMilliseconds > gazeTime)
            {
                if (IMG_Sound == true)
                {
                    string[] filePaths = Directory.GetFiles(path_question);
                    string extendsion_type = System.IO.Path.GetExtension(filePaths[Sound_Number]);
                    if (extendsion_type == ".txt")
                    {
                        string strTTS = "";
                        String TTS_txt_path = String.Format(path_question + @"/sound" + selectIndex + ".txt");
                        if (File.Exists(TTS_txt_path))
                        {
                            using (StreamReader reader = new StreamReader(TTS_txt_path))
                            {
                                strTTS = reader.ReadLine();
                            }
                        }
                        Say(strTTS);
                    }
                    else
                    {
                        string path_sound = string.Format(path_question + @"/sound" + selectIndex + System.IO.Path.GetExtension(filePaths[Sound_Number]));
                        soundPlayer_play.Open(new Uri(path_sound));
                        soundPlayer_play.Play();
                    }
                }
                else
                {
                    soundPlayer_play.Open(new Uri(@"Sounds/System/silence.mp3", UriKind.Relative));
                    soundPlayer_play.Play();
                }
                timer.IsEnabled = false;
                previous.IsEnabled = false;
                next.IsEnabled = false;
                soundPlayer_play.Play();
                inkCanvas_ER.Children.Add(rectangles[selectIndex]);

                List<Rectangle> lastRectangle = new List<Rectangle>();
                lastRectangle.Add(rectangles[selectIndex]);
                inkCanvas_ER.Select(lastRectangle);

                inkCanvas_ER.Children.Add(text_ER);

                CloseFullScreenHandler();
            }
        }
        private void SetGrayscale(System.Windows.Controls.Image img)
        {

            FormatConvertedBitmap bitmap = new FormatConvertedBitmap();
            bitmap.BeginInit();
            bitmap.Source = (BitmapSource)img.Source;
            bitmap.DestinationFormat = PixelFormats.Gray32Float;
            bitmap.EndInit();

            img.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => { img.Source = bitmap; }));
        }
        #endregion

        #region MouseControl

        private void MouseTrackBlock()
        {
            Point pointToWindow = Mouse.GetPosition(this);
            Point pointToScreen = PointToScreen(pointToWindow);
            if (pointToScreen.Y < screenHeight * 0.9 && pointToScreen.Y > screenHeight * 0.1)
            {
                if (Global.current1246 == 1)
                {
                    selectIndex = 1;
                }
                else if (Global.current1246 == 2)
                {
                    if (pointToScreen.X < screenWidth * 0.5 && pointToScreen.X > screenWidth * 0)
                    {
                        selectIndex = 1;
                    }
                    else if (pointToScreen.X < screenWidth * 1 && pointToScreen.X > screenWidth * 0.5)
                    {
                        selectIndex = 2;
                    }
                }
                else if (Global.current1246 == 4)
                {
                    if (pointToScreen.X < screenWidth * 0.5 && pointToScreen.X > screenWidth * 0 && pointToScreen.Y < screenHeight * 0.5 && pointToScreen.Y > screenHeight * 0)
                    {
                        selectIndex = 1;
                    }
                    else if (pointToScreen.X < screenWidth * 1 && pointToScreen.X > screenWidth * 0.5 && pointToScreen.Y < screenHeight * 0.5 && pointToScreen.Y > screenHeight * 0)
                    {
                        selectIndex = 2;
                    }
                    else if (pointToScreen.X < screenWidth * 0.5 && pointToScreen.X > screenWidth * 0 && pointToScreen.Y < screenHeight * 1 && pointToScreen.Y > screenHeight * 0.5)
                    {
                        selectIndex = 3;
                    }
                    else if (pointToScreen.X < screenWidth * 1 && pointToScreen.X > screenWidth * 0.5 && pointToScreen.Y < screenHeight * 1 && pointToScreen.Y > screenHeight * 0.5)
                    {
                        selectIndex = 4;
                    }
                }
                else if (Global.current1246 == 6)
                {
                    if (pointToScreen.X < screenWidth * 0.33 && pointToScreen.X > screenWidth * 0 && pointToScreen.Y < screenHeight * 0.5 && pointToScreen.Y > screenHeight * 0)
                    {
                        selectIndex = 1;
                    }
                    else if (pointToScreen.X < screenWidth * 0.66 && pointToScreen.X > screenWidth * 0.33 && pointToScreen.Y < screenHeight * 0.5 && pointToScreen.Y > screenHeight * 0)
                    {
                        selectIndex = 2;
                    }
                    else if (pointToScreen.X < screenWidth * 1 && pointToScreen.X > screenWidth * 0.66 && pointToScreen.Y < screenHeight * 0.5 && pointToScreen.Y > screenHeight * 0)
                    {
                        selectIndex = 3;
                    }
                    else if (pointToScreen.X < screenWidth * 0.33 && pointToScreen.X > screenWidth * 0 && pointToScreen.Y < screenHeight * 1 && pointToScreen.Y > screenHeight * 0.5)
                    {
                        selectIndex = 4;
                    }
                    else if (pointToScreen.X < screenWidth * 0.66 && pointToScreen.X > screenWidth * 0.33 && pointToScreen.Y < screenHeight * 1 && pointToScreen.Y > screenHeight * 0.5)
                    {
                        selectIndex = 5;
                    }
                    else if (pointToScreen.X < screenWidth * 1 && pointToScreen.X > screenWidth * 0.66 && pointToScreen.Y < screenHeight * 1 && pointToScreen.Y > screenHeight * 0.5)
                    {
                        selectIndex = 6;
                    }
                }
                else if (Global.current1246 == ER)
                {
                    selectIndex = -1;
                    for (int i = 0; i < rectangles.Count; i++)
                    {
                        double screen_ratio = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;
                        double X1 = InkCanvas.GetLeft(rectangles[i]) * screen_ratio;
                        double X2 = (InkCanvas.GetLeft(rectangles[i]) + rectangles[i].Width) * screen_ratio;
                        double Y1 = InkCanvas.GetTop(rectangles[i]) * screen_ratio;
                        double Y2 = (InkCanvas.GetTop(rectangles[i]) + rectangles[i].Height) * screen_ratio;
                        if (pointToScreen.X > X1 && pointToScreen.X < X2 && pointToScreen.Y > Y1 && pointToScreen.Y < Y2)
                        {
                            selectIndex = i;
                        }
                        //Console.WriteLine(X1 + ","+X2);

                    }
                    //Console.WriteLine(eyePos.X + "," + eyePos.Y);
                }
            }
            else
            {
                selectIndex = -1;
            }

        }
        private void MouseFocusVideo()
        {
            for (int i = 0; i < number_of_buttons; i++)
            {
                video_array[i].Pause();
            }
            video_array[selectIndex - 1].Play();
        }
        public void MouseFocusImage()
        {
            if (IMG_Sound == true)
            {
                string[] filePaths = Directory.GetFiles(path_question);
                string extendsion_type = System.IO.Path.GetExtension(filePaths[Sound_Number]);
                if (extendsion_type == ".txt")
                {
                    string strTTS = "";
                    String TTS_txt_path = String.Format(path_question + @"/sound" + selectIndex + ".txt");
                    if (File.Exists(TTS_txt_path))
                    {
                        using (StreamReader reader = new StreamReader(TTS_txt_path))
                        {
                            strTTS = reader.ReadLine();
                        }
                    }
                    Say(strTTS);
                }
                else
                {
                    string path_sound = string.Format(path_question + @"/sound" + selectIndex + System.IO.Path.GetExtension(filePaths[Sound_Number]));
                    soundPlayer_play.Open(new Uri(path_sound));
                    soundPlayer_play.Play();
                }
            }
            else
            {
                soundPlayer_play.Open(new Uri(@"Sounds/System/silence.mp3", UriKind.Relative));
                soundPlayer_play.Play();
            }
            //SetGrayscale(image_array[selectIndex - 1]);
            hover_border_array[selectIndex - 1].Visibility = Visibility.Visible;
            timer.IsEnabled = false;
            previous.IsEnabled = false;
            next.IsEnabled = false;
            CloseFullScreenHandler();
        }
        private void MouseFocus_ER()
        {
            if (IMG_Sound == true)
            {
                string[] filePaths = Directory.GetFiles(path_question);
                string extendsion_type = System.IO.Path.GetExtension(filePaths[Sound_Number]);
                if (extendsion_type == ".txt")
                {
                    string strTTS = "";
                    String TTS_txt_path = String.Format(path_question + @"/sound" + selectIndex + ".txt");
                    if (File.Exists(TTS_txt_path))
                    {
                        using (StreamReader reader = new StreamReader(TTS_txt_path))
                        {
                            strTTS = reader.ReadLine();
                        }
                    }
                    Say(strTTS);
                }
                else
                {
                    string path_sound = string.Format(path_question + @"/sound" + selectIndex + System.IO.Path.GetExtension(filePaths[Sound_Number]));
                    soundPlayer_play.Open(new Uri(path_sound));
                    soundPlayer_play.Play();
                }
            }
            else
            {
                soundPlayer_play.Open(new Uri(@"Sounds/System/silence.mp3", UriKind.Relative));
                soundPlayer_play.Play();
            }
            timer.IsEnabled = false;
            previous.IsEnabled = false;
            next.IsEnabled = false;
            soundPlayer_play.Play();

            inkCanvas_ER.Children.Clear();
            inkCanvas_ER.Children.Add(rectangles[selectIndex]);

            List<Rectangle> lastRectangle = new List<Rectangle>();
            lastRectangle.Add(rectangles[selectIndex]);
            inkCanvas_ER.Select(lastRectangle);

            inkCanvas_ER.Children.Add(text_ER);
            CloseFullScreenHandler();
        }


        #endregion

        #region Block

        private void EditStart(int blockNum)
        {
            edit_button_array[blockNum - 1].Visibility = Visibility.Hidden;
            image_array[blockNum - 1].Stretch = Stretch.Fill;
            image_array[blockNum - 1].Source = new BitmapImage(new Uri(@"Images/Editor/block/block_1_default.png", UriKind.Relative)); ;
            load_image_button_array[blockNum - 1].Visibility = Visibility.Visible;
            if (Global.current1246 != ER)
            {
                if (Global.current1246 == 1)
                    load_video_button_array[blockNum - 1].Visibility = Visibility.Visible;
                else
                    load_video_button_array[blockNum - 1].Visibility = Visibility.Collapsed;
            }
        }
        private void loadVideo(int fileNum)
        {
            OpenFileDialog openVideoFile = new OpenFileDialog();
            //openVideoFile.InitialDirectory = (Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            openVideoFile.Filter = "Video Files|*.AVI; *.MKV; *.MOV; *.MP4; *.MPG; *.MPEG; *.RMVB; *.WMV;";
            DialogResult result = openVideoFile.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string Filename = openVideoFile.FileName;
                string currentVideoPath = string.Format(path_question + @"/video" + fileNum + System.IO.Path.GetExtension(Filename));
                File.Copy(Filename, currentVideoPath);
                Thread.Sleep(300);
                LoadQuestion();
            }
        }
        private void loadImage(int fileNum)
        {
            OpenFileDialog openImageFile = new OpenFileDialog();
            //openImageFile.InitialDirectory = (Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            openImageFile.Filter = "Images | *.BMP; *.GIF; *.JPEG; *.JPG; *.JPS; *.PNG";
            DialogResult openImageResult = openImageFile.ShowDialog();
            if (openImageResult == System.Windows.Forms.DialogResult.OK)
            {
                string Filename = openImageFile.FileName;
                string currentImagePath = string.Format(path_question + @"/image" + fileNum + System.IO.Path.GetExtension(Filename));

                int resizeWidth = 1000;
                if (Global.current1246 == 1 || Global.current1246 == ER)
                    resizeWidth = 2000;
                ResizeImage(Filename, currentImagePath, resizeWidth, 0);

                //File.Copy(Filename, currentImagePath);
                Thread.Sleep(300);

                LoadQuestion();
            }
        }

        private void ResizeImage(string inputPath, string outputPath, int width, int height)
        {
            var bitmap = new BitmapImage();

            using (var stream = new FileStream(inputPath, FileMode.Open))
            {
                bitmap.BeginInit();
                bitmap.DecodePixelWidth = width;
                //bitmap.DecodePixelHeight = height;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }


        private void loadSound(int fileNum)
        {
            if (choose_sound_box_array[fileNum - 1].Visibility == Visibility.Collapsed || choose_sound_box_array[fileNum - 1].Visibility == Visibility.Hidden)
                choose_sound_box_array[fileNum - 1].Visibility = Visibility.Visible;
            else
                choose_sound_box_array[fileNum - 1].Visibility = Visibility.Collapsed;
            sound1246Num = fileNum;
            IMG_VID(sound1246Num);
        }
        private void delete1246(int fileNum)
        {
            string[] filePaths = Directory.GetFiles(path_question);

            if (IMG_VID(fileNum) == "image")
            {
                image_array[fileNum - 1].Source = null;
                string path_image = string.Format(path_question + @"/image" + fileNum + System.IO.Path.GetExtension(filePaths[Image_Number]));
                File.Delete(path_image);
                if (Global.current1246 == ER)
                {

                }
                else
                {
                    if (IMG_Sound == true)
                    {
                        string path_sound = string.Format(path_question + @"/sound" + fileNum + System.IO.Path.GetExtension(filePaths[Sound_Number]));
                        File.Delete(path_sound);
                    }
                }
            }
            else if (IMG_VID(fileNum) == "video")
            {
                video_array[fileNum - 1].Source = null;
                string path_video = string.Format(path_question + @"/video" + fileNum + System.IO.Path.GetExtension(filePaths[Video_Number]));
                File.Delete(path_video);

            }
            else
            {
                System.Windows.MessageBox.Show("delete1246 bug");
            }
            Thread.Sleep(300);
            LoadQuestion();
        }

        #endregion

        #region Question
        private void LoadQuestion()
        {
            Console.WriteLine(mode);
            string[] directoryPaths = Directory.GetDirectories(path_subject);
            questionLabel.Content = Global.currentQuestion + "/" + directoryPaths.Length;

            Renew_path();
            CheckCurrent1246();
            Renew_path();
            Renew_control_array();
            ShowBlocks();

            if (Directory.Exists(path_question))
            {
                for (int i = 1; i < number_of_buttons + 1; i++)
                {
                    if (IMG_VID(i) == "video")
                    {
                        string[] filePaths = Directory.GetFiles(path_question);
                        string path_video = string.Format(path_question + @"/video" + i + System.IO.Path.GetExtension(filePaths[Video_Number]));
                        image_array[i - 1].Source = null;
                        video_array[i - 1].Source = new Uri(path_video);
                        //var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                        //ffMpeg.GetVideoThumbnail(pathToVideoFile, thumbJpegStream, 5);
                        load_video_button_array[i - 1].Visibility = Visibility.Hidden;
                        load_image_button_array[i - 1].Visibility = Visibility.Hidden;
                        sound_button_array[i - 1].Visibility = Visibility.Hidden;
                        edit_button_array[i - 1].Visibility = Visibility.Hidden;
                        choose_sound_box_array[i - 1].Visibility = Visibility.Hidden;
                        if (mode == "editor")
                        {
                            delete1246_button_array[i - 1].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            delete1246_button_array[i - 1].Visibility = Visibility.Hidden;
                        }

                    }
                    else if (IMG_VID(i) == "image")
                    {
                        string[] filePaths = Directory.GetFiles(path_question);
                        string path_image = string.Format(path_question + @"/image" + i + System.IO.Path.GetExtension(filePaths[Image_Number]));

                        image_array[i - 1].Stretch = Stretch.Uniform;
                        image_array[i - 1].Source = BitmapFromUri(path_image);
                        load_image_button_array[i - 1].Visibility = Visibility.Hidden;
                        edit_button_array[i - 1].Visibility = Visibility.Hidden;
                        if (Global.current1246 != ER)
                        {
                            choose_sound_box_array[i - 1].Visibility = Visibility.Hidden;
                            video_array[i - 1].Source = null;
                            load_video_button_array[i - 1].Visibility = Visibility.Hidden;
                            if (mode == "editor")
                            {
                                if (IMG_Sound)
                                    sound_button_array[i - 1].Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Editor/buttons/sound.png", UriKind.Relative)));
                                else
                                    sound_button_array[i - 1].Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Editor/buttons/no_sound.png", UriKind.Relative)));
                                sound_button_array[i - 1].Visibility = Visibility.Visible;
                                delete1246_button_array[i - 1].Visibility = Visibility.Visible;
                            }
                            else
                            {
                                sound_button_array[i - 1].Visibility = Visibility.Hidden;
                                delete1246_button_array[i - 1].Visibility = Visibility.Hidden;
                            }
                        }
                    }
                    else if (IMG_VID(i) == "nothing")
                    {
                        if (mode == "editor")
                        {
                            edit_button_array[i - 1].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            edit_button_array[i - 1].Visibility = Visibility.Hidden;
                        }
                        load_image_button_array[i - 1].Visibility = Visibility.Hidden;
                        if (Global.current1246 != ER)
                        {
                            load_video_button_array[i - 1].Visibility = Visibility.Hidden;
                            sound_button_array[i - 1].Visibility = Visibility.Hidden;
                            delete1246_button_array[i - 1].Visibility = Visibility.Hidden;
                            choose_sound_box_array[i - 1].Visibility = Visibility.Hidden;
                        }

                        image_array[i - 1].Stretch = Stretch.Fill;
                        image_array[i - 1].Source = new BitmapImage(new Uri(@"Images/Editor/block/block_1_edit.png", UriKind.Relative)); ;
                        if (Global.current1246 != ER)
                            video_array[i - 1].Source = null;
                    }
                }

                if (Global.current1246 == ER)
                {
                    inkCanvas_ER.Children.Clear();
                    load_ER();
                    LoadSmallRect();
                    if (mode == "editor")
                    {
                        if (IMG_VID(1) == "image")
                        {
                            add_ER.Visibility = Visibility.Visible;
                            if (rectangles.Count != 0)
                            {
                                show_rectangles();
                                set_ER_buttons();
                            }
                        }
                        else
                        {
                            add_ER.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        add_ER.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    add_ER.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CheckCurrent1246()
        {
            string[] txtFilePaths = Directory.GetFiles(path_question, "*.txt");
            string txtFileName = System.IO.Path.GetFileNameWithoutExtension(txtFilePaths[0]);
            if (txtFileName == "1")
            {
                Global.current1246 = 1;
            }
            else if (txtFileName == "2")
            {
                Global.current1246 = 2;
            }
            else if (txtFileName == "4")
            {
                Global.current1246 = 4;
            }
            else if (txtFileName == "6")
            {
                Global.current1246 = 6;
            }
            else if (txtFileName == "ER")
            {
                Global.current1246 = ER;
            }


        }
        private void Renew_path()
        {
            path_1246 = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/1246/";
            path_subject = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/1246/" + Global.currentSubject;
            path_question = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/1246/" + Global.currentSubject + @"/" + Global.currentQuestion;
        }
        private void Renew_control_array()
        {
            video_1_1.Source = null;
            video_2_1.Source = null;
            video_2_2.Source = null;
            video_4_1.Source = null;
            video_4_2.Source = null;
            video_4_3.Source = null;
            video_4_4.Source = null;
            video_6_1.Source = null;
            video_6_2.Source = null;
            video_6_3.Source = null;
            video_6_4.Source = null;
            video_6_5.Source = null;
            video_6_6.Source = null;
            image_1_1.Source = null;
            image_2_1.Source = null;
            image_2_2.Source = null;
            image_4_1.Source = null;
            image_4_2.Source = null;
            image_4_3.Source = null;
            image_4_4.Source = null;
            image_6_1.Source = null;
            image_6_2.Source = null;
            image_6_3.Source = null;
            image_6_4.Source = null;
            image_6_5.Source = null;
            image_6_6.Source = null;
            image_ER.Source = null;
            if (Global.current1246 == 1)
            {
                video_array = new MediaElement[] { video_1_1, };
                image_array = new Image[] { image_1_1, };
                edit_button_array = new System.Windows.Controls.Button[] { edit_button_1_1, };
                load_video_button_array = new System.Windows.Controls.Button[] { loadVideo_button_1_1 };
                load_image_button_array = new System.Windows.Controls.Button[] { loadImage_button_1_1 };
                sound_button_array = new System.Windows.Controls.Button[] { sound_button_1_1 };
                delete1246_button_array = new System.Windows.Controls.Button[] { delete1246_button_1_1 };
                choose_sound_box_array = new System.Windows.Controls.StackPanel[] { choose_sound_box_1_1 };
                hover_border_array = new Rectangle[]{ hover_border_1_1 };
            }
            else if (Global.current1246 == 2)
            {
                video_array = new MediaElement[] { video_2_1, video_2_2, };
                image_array = new Image[] { image_2_1, image_2_2, };
                edit_button_array = new System.Windows.Controls.Button[] { edit_button_2_1, edit_button_2_2 };
                load_video_button_array = new System.Windows.Controls.Button[] { loadVideo_button_2_1, loadVideo_button_2_2 };
                load_image_button_array = new System.Windows.Controls.Button[] { loadImage_button_2_1, loadImage_button_2_2 };
                sound_button_array = new System.Windows.Controls.Button[] { sound_button_2_1, sound_button_2_2 };
                delete1246_button_array = new System.Windows.Controls.Button[] { delete1246_button_2_1, delete1246_button_2_2 };
                choose_sound_box_array = new System.Windows.Controls.StackPanel[] { choose_sound_box_2_1, choose_sound_box_2_2 };
                this.hover_border_array = new Rectangle[]{ this.hover_border_2_1, this.hover_border_2_2 };
            }
            else if (Global.current1246 == 4)
            {
                video_array = new MediaElement[] { video_4_1, video_4_2, video_4_3, video_4_4, };
                image_array = new Image[] { image_4_1, image_4_2, image_4_3, image_4_4, };
                edit_button_array = new System.Windows.Controls.Button[] { edit_button_4_1, edit_button_4_2, edit_button_4_3, edit_button_4_4 };
                load_video_button_array = new System.Windows.Controls.Button[] { loadVideo_button_4_1, loadVideo_button_4_2, loadVideo_button_4_3, loadVideo_button_4_4 };
                load_image_button_array = new System.Windows.Controls.Button[] { loadImage_button_4_1, loadImage_button_4_2, loadImage_button_4_3, loadImage_button_4_4 };
                sound_button_array = new System.Windows.Controls.Button[] { sound_button_4_1, sound_button_4_2, sound_button_4_3, sound_button_4_4 };
                delete1246_button_array = new System.Windows.Controls.Button[] { delete1246_button_4_1, delete1246_button_4_2, delete1246_button_4_3, delete1246_button_4_4 };
                choose_sound_box_array = new System.Windows.Controls.StackPanel[] { choose_sound_box_4_1, choose_sound_box_4_2, choose_sound_box_4_3, choose_sound_box_4_4 };
                this.hover_border_array = new Rectangle[4]{ this.hover_border_4_1, this.hover_border_4_2, this.hover_border_4_3, this.hover_border_4_4 };
            }
            else if (Global.current1246 == 6)
            {
                video_array = new MediaElement[] { video_6_1, video_6_2, video_6_3, video_6_4, video_6_5, video_6_6 };
                image_array = new Image[] { image_6_1, image_6_2, image_6_3, image_6_4, image_6_5, image_6_6 };
                edit_button_array = new System.Windows.Controls.Button[] { edit_button_6_1, edit_button_6_2, edit_button_6_3, edit_button_6_4, edit_button_6_5, edit_button_6_6 };
                load_video_button_array = new System.Windows.Controls.Button[] { loadVideo_button_6_1, loadVideo_button_6_2, loadVideo_button_6_3, loadVideo_button_6_4, loadVideo_button_6_5, loadVideo_button_6_6 };
                load_image_button_array = new System.Windows.Controls.Button[] { loadImage_button_6_1, loadImage_button_6_2, loadImage_button_6_3, loadImage_button_6_4, loadImage_button_6_5, loadImage_button_6_6 };
                sound_button_array = new System.Windows.Controls.Button[] { sound_button_6_1, sound_button_6_2, sound_button_6_3, sound_button_6_4, sound_button_6_5, sound_button_6_6 };
                delete1246_button_array = new System.Windows.Controls.Button[] { delete1246_button_6_1, delete1246_button_6_2, delete1246_button_6_3, delete1246_button_6_4, delete1246_button_6_5, delete1246_button_6_6 };
                choose_sound_box_array = new System.Windows.Controls.StackPanel[] { choose_sound_box_6_1, choose_sound_box_6_2, choose_sound_box_6_3, choose_sound_box_6_4, choose_sound_box_6_5, choose_sound_box_6_6 };
                this.hover_border_array = new Rectangle[6] { this.hover_border_6_1, this.hover_border_6_2, this.hover_border_6_3, this.hover_border_6_4, this.hover_border_6_5, this.hover_border_6_6 };
            }
            if (Global.current1246 == ER)
            {
                video_array = null;
                image_array = new Image[] { image_ER, };
                edit_button_array = new System.Windows.Controls.Button[] { edit_button_ER };
                load_video_button_array = null;
                load_image_button_array = new System.Windows.Controls.Button[] { loadImage_button_ER };
                sound_button_array = null;
                delete1246_button_array = null;
                choose_sound_box_array = null;
            }

            if (Global.current1246 == ER)
                number_of_buttons = 1;
            else
                number_of_buttons = Global.current1246;
        }
        private void ShowBlocks()
        {
            if (Global.current1246 == 1)
            {
                grid_one.Visibility = Visibility.Visible;
                grid_two.Visibility = Visibility.Collapsed;
                grid_four.Visibility = Visibility.Collapsed;
                grid_six.Visibility = Visibility.Collapsed;
                grid_ER.Visibility = Visibility.Collapsed;
                Shaddles_1.Visibility = Visibility.Visible;
                Shaddles_2.Visibility = Visibility.Collapsed;
                Shaddles_4.Visibility = Visibility.Collapsed;
                Shaddles_6.Visibility = Visibility.Collapsed;
            }
            else if (Global.current1246 == 2)
            {
                grid_two.Visibility = Visibility.Visible;
                grid_one.Visibility = Visibility.Collapsed;
                grid_four.Visibility = Visibility.Collapsed;
                grid_six.Visibility = Visibility.Collapsed;
                grid_ER.Visibility = Visibility.Collapsed;
                Shaddles_2.Visibility = Visibility.Visible;
                Shaddles_1.Visibility = Visibility.Collapsed;
                Shaddles_4.Visibility = Visibility.Collapsed;
                Shaddles_6.Visibility = Visibility.Collapsed;
            }
            else if (Global.current1246 == 4)
            {
                grid_four.Visibility = Visibility.Visible;
                grid_one.Visibility = Visibility.Collapsed;
                grid_two.Visibility = Visibility.Collapsed;
                grid_six.Visibility = Visibility.Collapsed;
                grid_ER.Visibility = Visibility.Collapsed;
                Shaddles_4.Visibility = Visibility.Visible;
                Shaddles_1.Visibility = Visibility.Collapsed;
                Shaddles_2.Visibility = Visibility.Collapsed;
                Shaddles_6.Visibility = Visibility.Collapsed;
            }
            else if (Global.current1246 == 6)
            {
                grid_six.Visibility = Visibility.Visible;
                grid_one.Visibility = Visibility.Collapsed;
                grid_two.Visibility = Visibility.Collapsed;
                grid_four.Visibility = Visibility.Collapsed;
                grid_ER.Visibility = Visibility.Collapsed;
                Shaddles_6.Visibility = Visibility.Visible;
                Shaddles_1.Visibility = Visibility.Collapsed;
                Shaddles_2.Visibility = Visibility.Collapsed;
                Shaddles_4.Visibility = Visibility.Collapsed;
            }
            else if (Global.current1246 == ER)
            {
                grid_one.Visibility = Visibility.Collapsed;
                grid_two.Visibility = Visibility.Collapsed;
                grid_four.Visibility = Visibility.Collapsed;
                grid_six.Visibility = Visibility.Collapsed;
                grid_ER.Visibility = Visibility.Visible;
                Shaddles_1.Visibility = Visibility.Visible;
                Shaddles_2.Visibility = Visibility.Collapsed;
                Shaddles_4.Visibility = Visibility.Collapsed;
                Shaddles_6.Visibility = Visibility.Collapsed;
            }
        }

        public static ImageSource BitmapFromUri(string source)
        {
            FileStream FStream = new FileStream(source, FileMode.Open);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = FStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            //bitmap.Freeze();
            FStream.Close();
            return bitmap;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
        }
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }
            Thread.Sleep(300);
            Directory.Delete(target_dir, false);
        }
        private void RenameQuestion_Delete(int number_of_directorys)
        {
            for (int i = Global.currentQuestion + 1; i < number_of_directorys + 1; i++)
            {
                timer.IsEnabled = false;
                Directory.Move(String.Format(path_subject + @"/{0}", i), String.Format(path_subject + @"/{0}", i - 1));
                timer.IsEnabled = true;
                //Thread.Sleep(1000);
            }
        }
        private void PreQuestion()
        {
            string[] directoryPaths = Directory.GetDirectories(path_subject);
            if (Global.currentQuestion == 1)
            {
                Global.currentQuestion = directoryPaths.Length;
            }
            else
            {
                Global.currentQuestion = Global.currentQuestion - 1;
            }
            LoadQuestion();
        }
        private void NextQuestion()
        {
            string[] directoryPaths = Directory.GetDirectories(path_subject);
            if (Global.currentQuestion == directoryPaths.Length)
            {
                Global.currentQuestion = 1;
            }
            else
            {
                Global.currentQuestion = Global.currentQuestion + 1;
            }
            LoadQuestion();
        }
        private void RenameQuestion_New()
        {
            string[] directoryPaths = Directory.GetDirectories(path_subject);
            for (int i = directoryPaths.Length; i > Global.currentQuestion; i--)
            {
                timer.IsEnabled = false;
                //video_array[0] = null;
                //video_1_1.Source = null;
                //video_1_1.Source = new Uri(@"C:\Users\Levi\Videos\4K Video Downloader\aaa.mp4", UriKind.Absolute);
                Directory.Move(String.Format(path_subject + @"/{0}", i), String.Format(path_subject + @"/{0}", i + 1));
                //DirectoryCopy(String.Format(path_subject + @"/{0}", i), String.Format(path_subject + @"/{0}", i + 1), false);
                //DeleteDirectory(path_question);

                timer.IsEnabled = true;
            }
        }

        
        #endregion

        #region Record

        int sound1246Num;
        Stopwatch stopwatch;
        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        System.Windows.Threading.DispatcherTimer timer_stopwatch;
        public void time_tick(object sender, EventArgs e)
        {
            TimeSpan ts = stopwatch.Elapsed;
            record_stopwatch.Text = string.Format(ts.ToString("mm\\:ss"));
        }

        #endregion

        #region EyeReality

        List<Rectangle> rectangles = new List<Rectangle>();
        int select_rect_index = -1;

        private void show_rectangles()
        {
            inkCanvas_ER.Children.Clear();
            foreach (var a in rectangles)
            {
                inkCanvas_ER.Children.Add(a);
            }
            inkCanvas_ER.Children.Add(sound_button_ER);
            inkCanvas_ER.Children.Add(delete_button_ER);
            inkCanvas_ER.Children.Add(soundBox_ER);
            inkCanvas_ER.Children.Add(text_ER);
            

            List<Rectangle> lastRectangle = new List<Rectangle>();
            lastRectangle.Add(rectangles.Last());
            inkCanvas_ER.Select(lastRectangle);
        }
        private bool isOverlap(Shape Rectangle1, Shape Rectangle2)
        {
            Rect rect1 = new Rect(InkCanvas.GetLeft(Rectangle1), InkCanvas.GetTop(Rectangle1), Rectangle1.Width, Rectangle1.Height);
            Rect rect2 = new Rect(InkCanvas.GetLeft(Rectangle2), InkCanvas.GetTop(Rectangle2), Rectangle2.Width, Rectangle2.Height);
            return rect1.IntersectsWith(rect2);
        }
        private bool isTooSmall(Shape rectangle)
        {
            if (rectangle.Width < 300 || rectangle.Height < 300)
                return true;
            else
                return false;
        }
        private void set_ER_buttons()
        {

            if (select_rect_index != -1)
            {
                IMG_VID(select_rect_index);
                if (IMG_Sound)
                    sound_button_ER.Style = (Style)TryFindResource("style_have_sound");
                else
                    sound_button_ER.Style = (Style)TryFindResource("style_no_sound");
                delete_button_ER.Visibility = Visibility.Visible;
                sound_button_ER.Visibility = Visibility.Visible;
                text_ER.Visibility = Visibility.Visible;
                soundBox_ER.Visibility = Visibility.Collapsed;

                double top_select_rect = InkCanvas.GetTop(rectangles[select_rect_index]);
                double left_select_rect = InkCanvas.GetLeft(rectangles[select_rect_index]);
                double height_select_rect = rectangles[select_rect_index].Height;
                double width_select_rect = rectangles[select_rect_index].Width;
                Console.WriteLine(top_select_rect + ", " + left_select_rect);

                InkCanvas.SetTop(delete_button_ER, top_select_rect + height_select_rect * 0.01);
                InkCanvas.SetLeft(delete_button_ER, left_select_rect + width_select_rect - delete_button_ER.Width - width_select_rect * 0.01);

                InkCanvas.SetTop(sound_button_ER, top_select_rect + height_select_rect * 0.01);
                InkCanvas.SetLeft(sound_button_ER, left_select_rect + width_select_rect * 0.01);

                InkCanvas.SetTop(soundBox_ER, top_select_rect + height_select_rect * 0.5 - 90);
                InkCanvas.SetLeft(soundBox_ER, left_select_rect + width_select_rect * 0.5 - 120);

                textbox_ER.Width = width_select_rect - 10;
                text_ER.Width = width_select_rect - 10;
                InkCanvas.SetTop(text_ER, top_select_rect + height_select_rect - text_ER.Height - 5);
                InkCanvas.SetLeft(text_ER, left_select_rect + width_select_rect * 0.5 - text_ER.Width * 0.5);

                //dragRect_ER.Width = width_select_rect;
                //dragRect_ER.Height = height_select_rect;
                //dragRect_ER.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 203, 139));
                //InkCanvas.SetTop(dragRect_ER, top_select_rect);
                //InkCanvas.SetLeft(dragRect_ER, left_select_rect);
            }
            else
            {
                delete_button_ER.Visibility = Visibility.Collapsed;
                sound_button_ER.Visibility = Visibility.Collapsed;
                soundBox_ER.Visibility = Visibility.Collapsed;
                text_ER.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveSmallRect()
        {
            if (mode == "fullscreen")
            {
                foreach (var rect in rectangles)
                {
                    InkCanvas.SetLeft(rect, InkCanvas.GetLeft(rect) * 7 / 9);
                    InkCanvas.SetTop(rect, InkCanvas.GetTop(rect) * 6 / 8);
                    rect.Width = rect.Width * 7 / 9;
                    rect.Height = rect.Height * 6 / 8;
                }
            }
            save_ER();
        }
        private void LoadSmallRect()
        {
            if (mode == "fullscreen")
            {
                foreach (var rect in rectangles)
                {
                    InkCanvas.SetLeft(rect, InkCanvas.GetLeft(rect) * 9 / 7);
                    InkCanvas.SetTop(rect, InkCanvas.GetTop(rect) * 8 / 6);
                    rect.Width = rect.Width * 9 / 7;
                    rect.Height = rect.Height * 8 / 6;
                }

            }
            save_ER();
        }

        private void ResizeRect()
        {
            if (mode == "fullscreen")
            {
                foreach (var rect in rectangles)
                {
                    InkCanvas.SetLeft(rect, InkCanvas.GetLeft(rect) * 9 / 7);
                    InkCanvas.SetTop(rect, InkCanvas.GetTop(rect) * 8 / 6);
                    rect.Width = rect.Width * 9 / 7;
                    rect.Height = rect.Height * 8 / 6;
                }

            }
            else if (mode == "editor")
            {
                foreach (var rect in rectangles)
                {
                    InkCanvas.SetLeft(rect, InkCanvas.GetLeft(rect) * 7 / 9);
                    InkCanvas.SetTop(rect, InkCanvas.GetTop(rect) * 6 / 8);
                    rect.Width = rect.Width * 7 / 9;
                    rect.Height = rect.Height * 6 / 8;
                }
                if (rectangles.Count != 0)
                    set_ER_buttons();
            }
            save_ER();
        }


        private string _ER_text;
        public string ER_text
        {
            get
            {
                return _ER_text;
            }
            set
            {
                if (value != _ER_text)
                {
                    _ER_text = value;
                    save_ER_text(_ER_text);
                    OnPropertyChanged("ER_text");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }


        private void save_ER_text(string input)
        {

            if (select_rect_index != -1)
            {
                string[] lines = System.IO.File.ReadAllLines(path_question + @"/ER_text.txt");
                lines[select_rect_index] = select_rect_index + "," + input;
                System.IO.File.WriteAllLines(path_question + @"/ER_text.txt", lines);
            }

        }

        string new_ER_text;
        private void load_ER_text()
        {
            if (File.Exists(path_question + @"/ER_text.txt"))
            {
                using (StreamReader reader = new StreamReader(path_question + @"/ER_text.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        int comma = line.IndexOf(",");


                        int rect_index = Convert.ToInt32(line.Substring(0, comma - 0));
                        string rect_text = line.Substring(comma + 1);
                        if (select_rect_index == rect_index)
                        {
                            new_ER_text = rect_text;
                        }
                    }
                }
                ER_text = new_ER_text;
            }
        }
        private void save_ER()
        {
            if (Global.current1246 == ER)
            {
                using (StreamWriter writer = new StreamWriter(path_question + @"/rectangles.txt", false))
                {
                    foreach (var rect in rectangles)
                    {
                        double top_rect = InkCanvas.GetTop(rect);
                        double left_rect = InkCanvas.GetLeft(rect);
                        double height_rect = rect.Height;
                        double width_rect = rect.Width;
                        writer.WriteLine(left_rect + "," + top_rect + "," + width_rect + "," + height_rect);
                    }
                }
            }
        }
        private void load_ER()
        {
            rectangles.Clear();
            //inkCanvas_ER.Children.Clear();
            if (File.Exists(path_question + @"/rectangles.txt"))
            {
                using (StreamReader reader = new StreamReader(path_question + @"/rectangles.txt"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        System.Console.WriteLine(line);
                        int comma1 = line.IndexOf(",");
                        int comma2 = line.IndexOf(",", comma1 + 1);
                        int comma3 = line.IndexOf(",", comma2 + 1);

                        double left_rect = Convert.ToDouble(line.Substring(0, comma1 - 0));
                        double top_rect = Convert.ToDouble(line.Substring(comma1 + 1, comma2 - comma1 - 1));
                        double width_rect = Convert.ToDouble(line.Substring(comma2 + 1, comma3 - comma2 - 1));
                        double height_rect = Convert.ToDouble(line.Substring(comma3 + 1));

                        Rectangle rectangle = new Rectangle();
                        InkCanvas.SetLeft(rectangle, left_rect);
                        InkCanvas.SetTop(rectangle, top_rect);
                        rectangle.Width = width_rect;
                        rectangle.Height = height_rect;
                        rectangle.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 203, 139));
                        rectangle.StrokeThickness = 5;
                        rectangle.Fill = new SolidColorBrush(Color.FromArgb(50, 255, 203, 139));
                        DoubleCollection aa = new DoubleCollection() { 4, 2 };
                        rectangle.StrokeDashArray = aa;

                        rectangle.MouseLeftButtonDown += new MouseButtonEventHandler(dragRect_ER_mouseDown);
                        rectangle.MouseMove += new System.Windows.Input.MouseEventHandler(dragRect_ER_mouseMove);
                        rectangle.MouseLeftButtonUp += new MouseButtonEventHandler(dragRect_ER_mouseUp);

                        rectangles.Add(rectangle);
                        //inkCanvas_ER.Children.Add(rectangle);
                    }
                }

            }
        }

        #endregion

        #region BarAnimation
        private void StartFullScreenHandler()
        {
            grid_all.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(grid_all_MouseDown), true);
            grid_all.MouseMove += grid_all_MouseMove;
            grid_all.MouseDown += grid_all_MouseDown;

        }

        private void CloseFullScreenHandler()
        {
            grid_all.RemoveHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(grid_all_MouseDown));
            grid_all.MouseMove -= grid_all_MouseMove;
            grid_all.MouseDown -= grid_all_MouseDown;

        }

        private void EnterBarAnimation()
        {
            var sb = new System.Windows.Media.Animation.Storyboard();

            var ta = new System.Windows.Media.Animation.ThicknessAnimation();
            ta.BeginTime = new TimeSpan(0);
            ta.SetValue(System.Windows.Media.Animation.Storyboard.TargetNameProperty, "topBar");
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(ta, new PropertyPath(MarginProperty));

            //ta.From = new Thickness(0, 0, 0, 80);
            ta.To = new Thickness(0, 0, 0, 0);
            ta.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            var ta2 = new System.Windows.Media.Animation.ThicknessAnimation();
            ta2.BeginTime = new TimeSpan(0);
            ta2.SetValue(System.Windows.Media.Animation.Storyboard.TargetNameProperty, "bottomBar");
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(ta2, new PropertyPath(MarginProperty));

            //ta2.From = new Thickness(0, 80, 0, 0);
            ta2.To = new Thickness(0, 0, 0, 0);
            ta2.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            var da = new System.Windows.Media.Animation.DoubleAnimation();
            da.BeginTime = new TimeSpan(0);
            da.SetValue(System.Windows.Media.Animation.Storyboard.TargetNameProperty, "base_buttons");
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(da, new PropertyPath(OpacityProperty));

            //da.From = 0;
            da.To = 1;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            sb.Children.Add(ta);
            sb.Children.Add(ta2);
            sb.Children.Add(da);
            sb.Begin(this);
        }

        System.Windows.Media.Animation.Storyboard sb = new System.Windows.Media.Animation.Storyboard();
        private void LeaveBarAnimation()
        {


            var ta = new System.Windows.Media.Animation.ThicknessAnimation();
            ta.BeginTime = new TimeSpan(0);
            ta.SetValue(System.Windows.Media.Animation.Storyboard.TargetNameProperty, "topBar");
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(ta, new PropertyPath(MarginProperty));

            //ta.From = new Thickness(0, 0, 0, 0);
            ta.To = new Thickness(0, 0, 0, 80);
            ta.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            var ta2 = new System.Windows.Media.Animation.ThicknessAnimation();
            ta2.BeginTime = new TimeSpan(0);
            ta2.SetValue(System.Windows.Media.Animation.Storyboard.TargetNameProperty, "bottomBar");
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(ta2, new PropertyPath(MarginProperty));

            //ta2.From = new Thickness(0, 0, 0, 0);
            ta2.To = new Thickness(0, 80, 0, 0);
            ta2.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            var da = new System.Windows.Media.Animation.DoubleAnimation();
            da.BeginTime = new TimeSpan(0);
            da.SetValue(System.Windows.Media.Animation.Storyboard.TargetNameProperty, "base_buttons");
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(da, new PropertyPath(OpacityProperty));
            base_buttons.Opacity = 0;
            //da.From = 100;
            da.To = 0;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            sb.Children.Add(ta);
            sb.Children.Add(ta2);
            sb.Children.Add(da);
            sb.Begin(this);
        }


        bool isEnterAnimation = false;
        private void grid_all_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mode == "editor")
            {
                CloseFullScreenHandler();
            }
            Point pointToWindow = Mouse.GetPosition(this);
            Point pointToScreen = PointToScreen(pointToWindow);
            if (pointToScreen.Y > screenHeight * 0.9 || pointToScreen.Y < screenHeight * 0.1)
            {
                if (isEnterAnimation == false)
                {
                    EnterBarAnimation();
                    isEnterAnimation = true;
                    timer.IsEnabled = false;
                }
            }
            else
            {
                if (isEnterAnimation == true)
                {
                    LeaveBarAnimation();
                    isEnterAnimation = false;
                    timer.IsEnabled = true;
                }
            }
        }

        private void grid_all_MouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MouseTrackBlock();
            if (Global.current1246 != ER)
            {
                if (IMG_VID(selectIndex) == "video")
                {
                    //MouseFocusVideo();
                }
                else if (IMG_VID(selectIndex) == "image")
                {
                    for (int i = 0; i < number_of_buttons; i++)
                    {
                        video_array[i].Pause();
                    }
                    MouseFocusImage();
                }
                else if (IMG_VID(selectIndex) == "nothing")
                {
                    for (int i = 0; i < number_of_buttons; i++)
                    {
                        if (Global.current1246 != ER)
                            video_array[i].Pause();
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("ThreadTimer Bug");
                }
            }
            else
            {
                IMG_VID(selectIndex);
                if (selectIndex != -1)
                    MouseFocus_ER();
            }
        }
        #endregion


    }

}
