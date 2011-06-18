using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace JewelBot
{
    static class WinApi
    {
        [DllImport("gdi32.dll")]
        public static extern int GetPixel(IntPtr hDC, int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public static T getFromDesktopDc<T>(Func<IntPtr, T> func)
        {
            T result = default(T);
            doWithDesktopDc(dc => result = func(dc));
            return result;
        }

        public static void doWithDesktopDc(Action<IntPtr> action)
        {
            var dc = GetDC((IntPtr)0);
            try { action(dc); }
            finally
            {
                ReleaseDC((IntPtr)0, dc);
            }
        }
    }
}
