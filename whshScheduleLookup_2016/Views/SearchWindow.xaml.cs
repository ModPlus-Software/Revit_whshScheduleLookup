using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using ModPlusAPI.Windows.Helpers;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using SearchViewModel = whshScheduleLookup.ViewModels.SearchViewModel;

namespace whshScheduleLookup.Views
{

    public partial class SearchWindow : MetroWindow
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
            this.OnWindowStartUp();
            _thisWindowIntPtr = new WindowInteropHelper(this).Handle;
            DataContext = searchViewModel;

            Process[] processes = Process.GetProcessesByName("Revit");
            if (0 < processes.Length) _revitWindowPtr = processes[0].MainWindowHandle;
        }

        private void MetroWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            Opacity = 1;
        }

        private void MetroWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            //this.Opacity = 0.1;
            Opacity = 1;
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            //Properties.Settings.Default.Save();
            Close();
        }

        private void TopMostMetroWindow_OnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Escape)
            {
                Close();
            }
            //ResetProgressBar();
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
            //this.StateLabel.Foreground = new SolidColorBrush(Colors.Black);
            //Binding binding = new Binding();
            //binding.Path = new PropertyPath("FoundResults.Count");
            //binding.Mode = BindingMode.OneWay;
            //binding.Converter = new IntToBrushConverter();
            //this.StateLabel.SetBinding(Label.ForegroundProperty, binding);
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

        private void HeaderRadioButton_Click(object sender, RoutedEventArgs e)
        {
            ResetProgressBar();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
