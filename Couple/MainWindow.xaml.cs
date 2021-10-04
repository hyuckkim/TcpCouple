using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Couple
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Label log;
        public static string windowPoint;
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new();

            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
            log = console;
            try
            {
                TcpServer.Listen();
            }
            catch
            {
                TcpUser.Connect();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Point pos = PointToScreen(new Point(0, 0));
            double? rotation = null;

            windowPoint = pos.X + "," + pos.Y + '\0';
            if (TcpServer.running)
            {
                TcpServer.CheckNewClient();
                string data = TcpServer.CheckClientData();
                if (data is not null)
                {
                    var enPos = SplitPointString(data);
                    rotation = getDegree(enPos, pos);
                }
            }
            if (TcpUser.task is not null && (TcpUser.task?.IsCompleted ?? false))
            {
                string result = Encoding.ASCII.GetString(TcpUser.buffer);
                console.Content = result;
                TcpUser.task = null;
                var enPos = SplitPointString(result);
                rotation = getDegree(enPos, pos);
                TcpUser.SendToServer(pos.X + "," + pos.Y + '\0');
            }
            if (rotation.HasValue)
            {
                arrow.RenderTransform = new RotateTransform(rotation.Value, 200, 200);
            }
        }
        Point SplitPointString(string str)
        {
            try
            {
                var pos = str.Split(',');
                return new Point(int.Parse(pos[0]), int.Parse(pos[1]));
            }
            catch
            {
                throw new Exception($"cannot cut {str}.");
            }
        }
        double getDegree(double y, double x)
        {
            return Math.Atan2(y, x) * 180 / Math.PI;
        }
        double getDegree(Point p)
        {
            return getDegree(p.Y, p.X);
        }
        double getDegree(Point p1, Point p2)
        {
            return getDegree(
                p1.Y - p2.Y,
                p1.X - p2.X
                );
        }
    }
}
