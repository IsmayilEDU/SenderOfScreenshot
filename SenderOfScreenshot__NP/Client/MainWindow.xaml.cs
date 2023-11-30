using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var client = new Socket(AddressFamily.InterNetwork,
                        SocketType.Dgram,
                        ProtocolType.Udp);

            var ip = IPAddress.Parse("10.10.144.177");
            var port = 27001;
            var remoteEP = new IPEndPoint(ip, port);

            var buffer = Array.Empty<byte>();

            buffer = Encoding.Default.GetBytes("screenshot");
            client.SendTo(buffer, remoteEP);
            int CountBytes = 0;
            while (true)
            {
                var result = await client.ReceiveFromAsync(new ArraySegment<byte>(buffer),
                                                     SocketFlags.None,
                                                     remoteEP);
                CountBytes = result.ReceivedBytes;
                Image image = null;
                var ListOfImageBytes = new List<byte>();

                using (var ms = new MemoryStream())
                {
                    var TotalBytes = ms.ToArray().ToList();
                    CountBytes = TotalBytes.Count;
                    var BufferSize = ushort.MaxValue - 29;
                    while (CountBytes > 0)
                    {
                        if (CountBytes < BufferSize)
                        {
                            BufferSize = CountBytes;
                        }
                        var CurrentlyBuffer = new byte[BufferSize];
                        CurrentlyBuffer = TotalBytes.Skip(0).Take(BufferSize - 1).ToArray();
                        ListOfImageBytes.AddRange(CurrentlyBuffer);
                        TotalBytes.RemoveRange(0, BufferSize);
                        CountBytes -= BufferSize;
                    }

                    image_Screenshot.Source = GetBitmapImageFromBytes(ListOfImageBytes.ToArray());

                }

            }
        }


        private BitmapImage GetBitmapImageFromBytes(byte[] imageBytes)
        {
            using (var imageStream = new MemoryStream(imageBytes))
            {
                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption= BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = imageStream;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        
    }
}
