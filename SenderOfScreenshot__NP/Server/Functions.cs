using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal static class Functions
    {
        public static Bitmap GetScreenshot()
        {
            using Bitmap screenshot = new Bitmap(Console.WindowWidth, Console.WindowHeight);

            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(Console.WindowLeft, Console.WindowTop, 0, 0, screenshot.Size);
            }

            screenshot.Save("screenshot.png", ImageFormat.Png);

            return screenshot;
        }

        
    }
}
