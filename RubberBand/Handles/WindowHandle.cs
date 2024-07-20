using System;
using System.Windows.Forms;

namespace SCADtools.Revit.UI.Handles
{
    internal class WindowHandle : IWin32Window
    {
        private IntPtr m_hwnd;

        public WindowHandle(IntPtr hwnd)
        {
            m_hwnd = hwnd;
        }

        public IntPtr Handle => m_hwnd;
    }
}
