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
using System.Windows.Shapes;
using System.Threading;

using EyeXFramework;
using Tobii.EyeX.Framework;
using EyeXFramework.Wpf;


namespace RettSydrome
{
    public partial class EyeCanvas : Window
    {
        public Boolean b_switch = true;
        double[] cursor_switch = new double[3];
        double[] eye_switch = new double[3];
        Thread dot;
        public Ellipse ellipse;
        double eyex, eyey;
        public System.Windows.Threading.DispatcherTimer d_timer, m_timer;
        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
        int i = 0;
        int j = 0;
        //public readonly WpfEyeXHost _eyeXHost;
        public readonly WpfEyeXHost eHost = new WpfEyeXHost();
        Point point;
        double ellipseR = Global.ellipseSize;
        int cursorPosX, cursorPosY;
        static int smoothQuantity = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Smooth"]);
        //ThreadingTimer _ThreadTimer = null;
        //TimersTimer _TimersTimer = null;
        public EyeCanvas()
        {
            InitializeComponent();
            //Mouse.OverrideCursor = Cursors.None;
            //_eyeXHost = new WpfEyeXHost();
            eHost = new WpfEyeXHost();
        }
        public void MouseHide()
        {
            if (Mouse.OverrideCursor == Cursors.None)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (Mouse.OverrideCursor == Cursors.Arrow)
            {
                Mouse.OverrideCursor = Cursors.None;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            eHost.Start();
            GazePointDataStream gpda = eHost.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered);
            gpda.Next += new EventHandler<GazePointEventArgs>(GetPos);

            eHost.GazeTrackingChanged += new EventHandler<EngineStateValue<GazeTracking>>(GetGaze);

            //dot = new Thread(Eyetracking);
            ellipse = new Ellipse();

            mySolidColorBrush.Color = Color.FromArgb(150, 250, 250, 250);
            ellipse.Fill = mySolidColorBrush;
            //邊框
            ellipse.HorizontalAlignment = HorizontalAlignment.Center;
            ellipse.VerticalAlignment = VerticalAlignment.Center;
            //ellipse.StrokeThickness = 3;
            //ellipse.Stroke = Brushes.Black;
            ellipse.Width = ellipseR;
            ellipse.Height = ellipseR;
            this.EyeEllipseCanvas.Children.Add(ellipse);
            //dot.Start();

            d_timer = new System.Windows.Threading.DispatcherTimer();
            d_timer.Tick += new EventHandler(DrawDot);
            d_timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            d_timer.IsEnabled = true;

        }

        public void StartDrawDot()
        {
            d_timer.IsEnabled = true;
        }

        public void StopDrawDot()
        {
            d_timer.IsEnabled = false;
            ellipse.Visibility = Visibility.Hidden;
        }

        Point[] eyePos = new Point[smoothQuantity];
        private Point smoothEyeGaze(Point pos)
        {
            if (eyePos[0] != pos)
            {
                for (int i = 0; i < eyePos.Length - 1; i++)
                {
                    eyePos[eyePos.Length - i - 1] = eyePos[eyePos.Length - i - 2];
                }
                eyePos[0] = pos;
            }
            Point smoothPos = new Point(0, 0);
            int count = 0;
            foreach (Point p in eyePos)
            {
                if (p.X > 0 && p.Y > 0)
                {
                    smoothPos.X += p.X;
                    smoothPos.Y += p.Y;
                    count++;
                }
            }
            return new Point(smoothPos.X / count, smoothPos.Y / count);
        }
        bool once = true;
        public void DrawDot(object sender, EventArgs e)
        {
            Canvas.SetLeft(ellipse, eyePoint.X * SystemParameters.PrimaryScreenWidth / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - ellipse.Width / 2);
            Canvas.SetTop(ellipse, eyePoint.Y * SystemParameters.PrimaryScreenWidth / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - ellipse.Height / 2);
            eye_switch[j] = eyex + eyey;
            j++;
            if (j == 3) j = 0;
            if (eye_switch[0] != eye_switch[1] && eye_switch[1] != eye_switch[2])
            {
                if (b_switch) ellipse.Visibility = Visibility.Visible;
                once = true;
                //SetCursorPos((int)eyePoint.X, (int)eyePoint.Y);
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else
            {
                if (b_switch) ellipse.Visibility = Visibility.Hidden;
                if (once)
                {
                    //SetCursorPos(0, 0);
                    once = false;
                }
                //Mouse.OverrideCursor = Cursors.None;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);
        Point eyePoint = new Point(0, 0);

        private void GetPos(object sender, GazePointEventArgs e)
        {
            eyex = e.X;
            eyey = e.Y;
            eyePoint = smoothEyeGaze(new Point(eyex, eyey));
        }

        //public void Eyetracking()
        //{
        //    using (_eyeXHost)
        //    {
        //        _eyeXHost.Start();

                

        //        using (var lightlyFilteredGazeDataStream = _eyeXHost.CreateGazePointDataStream(GazePointDataMode.LightlyFiltered))
        //        {
        //            _eyeXHost.GazeTrackingChanged += new EventHandler<EngineStateValue<GazeTracking>>(GetGaze);

        //            lightlyFilteredGazeDataStream.Next += (s, e) =>
        //            {

        //                //Console.WriteLine("Gaze point at ({0:0.0}, {1:0.0}) @{2:0}", e.X, e.Y, e.Timestamp);
        //                eyex = e.X;
        //                eyey = e.Y;
        //                //Point tempEye = new Point(eyex, eyey);
        //                eyePoint = smoothEyeGaze(new Point(eyex, eyey));
        //                //SetCursorPos((int)smoothEyeGaze(tempEye).X, (int)smoothEyeGaze(tempEye).Y);

        //            };// Let it run until a key is pressed.
        //            //Console.WriteLine("Listening for gaze data, press any key to exit...");
        //            Console.In.Read();
        //        }

                
        //    }
        //}


        Boolean isGazeTracked = false;

        public void GetGaze(object sender, EngineStateValue<GazeTracking> e)
        {
            if (eHost.GazeTracking.ToString() == "GazeNotTracked")
            {
                isGazeTracked = false;
            }
            if (eHost.GazeTracking.ToString() == "GazeTracked")
            {
                isGazeTracked = true;
            }
        }

        public Point GetEyePoint()
        {
            return eyePoint;
        }

        public Boolean GetGazeTracked()
        {
            return isGazeTracked;
        }

        public void Zoom(double size)
        {
            ellipse.Width = size;
            ellipse.Height = size;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                b_switch = false;
                closeAll();
            }
        }
        public void closeAll()
        {
            this.Close();
            try
            {
                Application.Current.Shutdown();
                System.Environment.Exit(System.Environment.ExitCode);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("catch close error : " + ex.ToString());
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            eHost.Dispose();
        }

    }
}
