using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Navigation;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using SearchViewModel = whshScheduleLookup.ViewModels.SearchViewModel;

namespace whshScheduleLookup.Views
{

    public partial class SearchWindow 
    {
        private IntPtr _thisWindowIntPtr;
        private readonly IntPtr _revitWindowPtr;
        /// <summary>
        /// The GetForegroundWindow function returns a 
        /// handle to the foreground window.
        /// </summary>
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Move the window associated with the passed 
        /// handle to the front.
        /// </summary>
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public SearchWindow(SearchViewModel searchViewModel)
        {
            InitializeComponent();
            
            _thisWindowIntPtr = new WindowInteropHelper(this).Handle;
            DataContext = searchViewModel;

            Process[] processes = Process.GetProcessesByName("Revit");
            if (0 < processes.Length) _revitWindowPtr = processes[0].MainWindowHandle;
        }
        
        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (DataContext is SearchViewModel vm && 
                e.Source is TextBox textBox && 
                textBox.SelectionLength < 1 && 
                textBox.Text.Contains("  "))
            {
                textBox.Text = textBox.Text.Trim() + vm.Delimeter + " "; // "; ";
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            ResetProgressBar();
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            Topmost = true;
            //ResetProgressBar();
            try
            {
                IntPtr hBefore = GetForegroundWindow();
                SetForegroundWindow(_revitWindowPtr);
                SetForegroundWindow(hBefore);
            }
            catch
            {
                //ignored
            }
            Topmost = false;
        }

        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void ResetProgressBar()
        {
            ProgressBar.Value = 0;
        }

        private void DelimiterTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ResetProgressBar();
        }

        private void CaseCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ResetProgressBar();
        }

        private void SubstringCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ResetProgressBar();
        }

        private void ValueRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ResetProgressBar();
        }

        private void ColumnRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ResetProgressBar();
        }

        private void FieldRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ResetProgressBar();
        }
    }
}
