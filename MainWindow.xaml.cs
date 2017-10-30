using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;

namespace RettSydrome
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void mydalegate();
        EditorWindow editorWindow;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        CheckSerialNumber checkSN;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            checkSN = new CheckSerialNumber();
            if (checkSN.Initialize())
            {
                if (!checkSN.CheckDefaultSN())
                {
                    System.Windows.MessageBox.Show("請使用軟體授權之眼動棒");
                    System.Windows.Application.Current.Shutdown();
                    return;
                }
                else
                {
                    string path_document = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (!File.Exists(path_document + @"/1246"))
                        Directory.CreateDirectory(path_document + @"/1246");

                    editorWindow = new EditorWindow();
                    this.Focus();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("請插入眼動棒");
                System.Windows.Application.Current.Shutdown();
                return;
            }

        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //    if (e.Key == Key.Escape)
            //    {
            //        this.Close();
            //        try
            //        {
            //            Application.Current.Shutdown();
            //            System.Environment.Exit(System.Environment.ExitCode);
            //            Environment.Exit(0);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine("catch close error : " + ex.ToString());
            //        }
            //    }
        }

        #region NewSubject
        private void new_subject_button_Click(object sender, RoutedEventArgs e)
        {
            new_subject_box.Visibility = Visibility.Visible;
            new_subject_button.Visibility = Visibility.Collapsed;
            open_subject_button.Visibility = Visibility.Collapsed;
            this.Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Home/homeBg2.jpg", UriKind.Relative)));
        }

        private void new_subject_Yes_Click(object sender, RoutedEventArgs e)
        {
            String input = InputTextBox.Text;
            if (string.IsNullOrWhiteSpace(input))
            {
                message_box2.Visibility = Visibility.Visible;
            }
            else
            {
                new_subject_box.Visibility = System.Windows.Visibility.Collapsed;
                String path = String.Format(path_1246 + @"/{0}", input);
                Directory.CreateDirectory(path);
                Global.currentSubject = input;
                InputTextBox.Text = String.Empty;
                ReloadSubject();
                editorWindow.Show();
                this.Close();
            }
        }

        private void new_subject_No_Click(object sender, RoutedEventArgs e)
        {
            new_subject_box.Visibility = System.Windows.Visibility.Collapsed;
            this.Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Home/homeBg1.jpg", UriKind.Relative)));
            new_subject_button.Visibility = Visibility.Visible;
            open_subject_button.Visibility = Visibility.Visible;
            InputTextBox.Text = String.Empty;
        }
        #endregion

        #region ChooseSubject
        private void open_subject_button_Click(object sender, RoutedEventArgs e)
        {

            string[] directoryPaths = Directory.GetDirectories(path_1246);

            int number_of_Directories = directoryPaths.Length;

            if (number_of_Directories != 0)
            {
                new_subject_button.Visibility = Visibility.Collapsed;
                open_subject_button.Visibility = Visibility.Collapsed;
                open_subject_box.Visibility = Visibility.Visible;
                this.Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Home/homeBg3.jpg", UriKind.Relative)));
            }
            else
            {
                message_box1.Visibility = Visibility.Visible;
            }
        }

        private void open_subject_Yes_Click(object sender, RoutedEventArgs e)
        {
            if (subject_ListView.SelectedItem != null)
            {
                open_subject_box.Visibility = System.Windows.Visibility.Collapsed;
                editorWindow.Show();
                this.Close();
            }
            else
            {
                message_box3.Visibility = Visibility.Visible;
            }
        }

        private void open_subject_No_Click(object sender, RoutedEventArgs e)
        {
            subject_ListView.SelectedItem = null;
            open_subject_box.Visibility = System.Windows.Visibility.Collapsed;
            new_subject_button.Visibility = Visibility.Visible;
            open_subject_button.Visibility = Visibility.Visible;
            this.Background = new ImageBrush(new BitmapImage(new Uri(@"Images/Home/homeBg1.jpg", UriKind.Relative)));
        }

        private void open_subject_Delete_Click(object sender, RoutedEventArgs e)
        {

            if (subject_ListView.SelectedItem != null)
            {
                delete_subject_box.Visibility = Visibility.Visible;
            }
            else
            {
                message_box3.Visibility = Visibility.Visible;
            }
        }

        private void open_subject_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (subject_ListView.SelectedItem != null)
            {
                edit_subject_box.Visibility = Visibility.Visible;
                editTextBox.Text = subject_ListView.SelectedItem.ToString();
            }
            else
            {
                message_box3.Visibility = Visibility.Visible;
            }
        }

        private void edit_subject_Yes_Click(object sender, RoutedEventArgs e)
        {
            if (editTextBox.Text != (string)subject_ListView.SelectedItem)
            {
                Directory.Move(path_1246 + @"/" + subject_ListView.SelectedItem, path_1246 + @"/" + editTextBox.Text);
            }
            edit_subject_box.Visibility = Visibility.Collapsed;
            ReloadSubject();
        }

        private void edit_subject_No_Click(object sender, RoutedEventArgs e)
        {
            edit_subject_box.Visibility = Visibility.Collapsed;
        }

        private void open_subject_In_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open1246File = new OpenFileDialog();
            open1246File.InitialDirectory = (Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            open1246File.Filter = "1246 Files|*.1246;";
            DialogResult result = open1246File.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string path = path_1246 + @"/" + System.IO.Path.GetFileNameWithoutExtension(open1246File.FileName);
                if (Directory.Exists(path))
                {
                    message_box4.Visibility = Visibility.Visible;
                }
                else
                {
                    Directory.CreateDirectory(path);
                    ZipFile.ExtractToDirectory(open1246File.FileName, path);
                    ReloadSubject();
                    //DirectoryCopy(open1246File.FileName, path_1246, true);
                }

            }
        }

        private void open_subject_Out_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog chooseFolder = new FolderBrowserDialog();
            String path_selected_subject = String.Format(path_1246 + @"/{0}", Global.currentSubject);

            DialogResult result = chooseFolder.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                String path_choose_folder_file = String.Format(chooseFolder.SelectedPath + @"/{0}" + ".1246", Global.currentSubject);
                if (File.Exists(path_choose_folder_file))
                {
                    message_box4.Visibility = Visibility.Visible;
                }
                else
                {
                    ZipFile.CreateFromDirectory(path_selected_subject, path_choose_folder_file);
                    //DirectoryCopy(path_selected_subject, path_choose_folder_file, true);
                }
            }
        }

        private void delete_subject_Yes_Click(object sender, RoutedEventArgs e)
        {
            Directory.Delete(path_1246 + @"/" + Global.currentSubject, true);
            Thread.Sleep(300);
            ReloadSubject();
            delete_subject_box.Visibility = Visibility.Collapsed;
        }

        private void delete_subject_No_Click(object sender, RoutedEventArgs e)
        {
            delete_subject_box.Visibility = Visibility.Collapsed;
        }
        private void subject_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as System.Windows.Controls.ListView;
            string value = listView.SelectedItem as string;
            Global.currentSubject = value;
            //Global.currentQuestion = 1;

            //win_Two.homeWindow = this;
            //win_Two.EC = EC;
        }

        string path_1246 = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/1246";

        private void subject_ListView_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadSubject();
        }

        private void ReloadSubject()
        {
            string[] directoryPaths = Directory.GetDirectories(path_1246);

            int number_of_Directories = directoryPaths.Length;

            if (number_of_Directories != 0)
            {
                List<string> fileNameList = new List<string>();
                foreach (string arrItem in directoryPaths)
                {
                    string fileName = System.IO.Path.GetFileName(arrItem);
                    fileNameList.Add(fileName);
                }

                subject_ListView.ItemsSource = fileNameList;
            }
            else
            {
                subject_ListView.ItemsSource = null;
            }
        }
        #endregion

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
        private void close_app_No_Click(object sender, RoutedEventArgs e)
        {
            close_app.Visibility = Visibility.Collapsed;
        }

        private void message_box1_Yes_Click(object sender, RoutedEventArgs e)
        {
            message_box1.Visibility = Visibility.Collapsed;
        }

        private void message_box2_Yes_Click(object sender, RoutedEventArgs e)
        {
            message_box2.Visibility = Visibility.Collapsed;
        }

        private void message_box3_Yes_Click(object sender, RoutedEventArgs e)
        {
            message_box3.Visibility = Visibility.Collapsed;
        }

        private void message_box4_Yes_Click(object sender, RoutedEventArgs e)
        {
            message_box4.Visibility = Visibility.Collapsed;
        }

        //private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        //{
        //    // Get the subdirectories for the specified directory.
        //    DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        //    if (!dir.Exists)
        //    {
        //        throw new DirectoryNotFoundException(
        //            "Source directory does not exist or could not be found: "
        //            + sourceDirName);
        //    }

        //    DirectoryInfo[] dirs = dir.GetDirectories();
        //    // If the destination directory doesn't exist, create it.
        //    if (!Directory.Exists(destDirName))
        //    {
        //        Directory.CreateDirectory(destDirName);
        //    }

        //    // Get the files in the directory and copy them to the new location.
        //    FileInfo[] files = dir.GetFiles();
        //    foreach (FileInfo file in files)
        //    {
        //        string temppath = System.IO.Path.Combine(destDirName, file.Name);
        //        file.CopyTo(temppath, false);
        //    }

        //    if (copySubDirs)
        //    {
        //        foreach (DirectoryInfo subdir in dirs)
        //        {
        //            string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
        //            DirectoryCopy(subdir.FullName, temppath, copySubDirs);
        //        }
        //    }
        //}

        
    }
}
