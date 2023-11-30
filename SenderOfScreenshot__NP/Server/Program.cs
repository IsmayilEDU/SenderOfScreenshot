using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

var ServerListener = new Socket(AddressFamily.InterNetwork,
                          SocketType.Dgram,
                          ProtocolType.Udp);


var ip = IPAddress.Parse("10.10.144.177");
var port = 27001;
var serverEP = new IPEndPoint(ip, port);

ServerListener.Bind(serverEP);

var buffer = new byte[ushort.MaxValue - 29];
EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

var CountBytes = 0;
var msg = string.Empty;

while (true)
{
    var result = await ServerListener.ReceiveFromAsync(new ArraySegment<byte>(buffer),
                                                 SocketFlags.None,
                                                 remoteEP);

    CountBytes = result.ReceivedBytes;
    msg = Encoding.Default.GetString(buffer, 0, CountBytes);
    if (msg == "screenshot")
    {
        while (true)
        {
            using Bitmap screenshot = new Bitmap(Console.WindowWidth, Console.WindowHeight);
            using (var ms = new MemoryStream())
            {
                screenshot.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
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
                    ServerListener.SendToAsync(CurrentlyBuffer,SocketFlags.None, remoteEP);
                    TotalBytes.RemoveRange(0, BufferSize);
                    CountBytes -= BufferSize; ;
                }
            }
        }

    }
}