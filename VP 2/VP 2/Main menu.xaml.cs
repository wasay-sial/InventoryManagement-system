using System;
using System.Windows;

namespace VP_2
{
    public partial class Main_menu : Window
    {
        public Main_menu()
        {
            InitializeComponent();
        }

        // Event handler for "Show Products" button
        private void ShowProductsButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();
            window.Show();
        }

        // Event handler for "Show Users" button
        private void ShowUsersButton_Click(object sender, RoutedEventArgs e)
        {
           Window2 window = new Window2(); 
           window.Show();
        }

        // Event handler for "Exit" button
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the application
            Application.Current.Shutdown();
        }
    }
}
