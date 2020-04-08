namespace whshScheduleLookup.Views
{
    using System.Windows;
    using System.Windows.Input;
    using ViewModels;

    public partial class YesNoCancelWindow
    {
        public YesNoCancelWindow()
        {
            InitializeComponent();
        }

        public YesNoCancelWindow(MessageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MessageViewModel vm) { vm.Result = true; }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MessageViewModel vm) { vm.Result = null; }
        }

        private void MetroWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (DataContext is MessageViewModel vm) { vm.Result = false; }
            }
            Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MessageViewModel vm) { vm.Result = false; }
            Close();
        }
    }
}
