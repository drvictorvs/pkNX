using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace pkNX.WinForms.Controls
{
    public abstract class AreaControls : UserControl
    {
        // Declare the LoadTable method as abstract
        public abstract void LoadTable<T>(IList<T> table, string path);
        protected abstract void Dispose(bool disposing);
    }
}
// 