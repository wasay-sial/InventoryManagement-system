using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;

namespace VP_2
{
    public partial class Window1 : Window
    {
        private string connectionString = "Data Source=WASAYPC;Initial Catalog=\"vp proje\";Integrated Security=True;Encrypt=False;";
        public ObservableCollection<Product> Products { get; set; }

        public Window1()
        {
            InitializeComponent();
            Products = new ObservableCollection<Product>();
            productDataGrid.ItemsSource = Products;
            LoadProducts();
        }

        private void LoadProducts()
        {
            Products.Clear();
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductId, ProductName, Quantity, Price, CategoryId FROM Products";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Products.Add(new Product
                        {
                            ProductId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            ProductName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Quantity = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                            Price = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                            CategoryId = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchBox.Text.ToLower();  // Get the search term from the search box
            var filteredProducts = new ObservableCollection<Product>();

            // Filter products based on the search term (ProductName)
            foreach (var product in Products)
            {
                if (product.ProductName.ToLower().Contains(searchTerm))
                {
                    filteredProducts.Add(product);
                }
            }

            // Update the DataGrid with filtered results
            productDataGrid.ItemsSource = filteredProducts;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new ProductInputDialog();
            var result = inputDialog.ShowDialog();

            if (result == true)
            {
                var newProduct = new Product
                {
                    ProductId = inputDialog.ProductId,
                    ProductName = inputDialog.ProductName,
                    Quantity = inputDialog.Quantity,
                    Price = inputDialog.Price,
                    CategoryId = inputDialog.CategoryId
                };

                Products.Add(newProduct);

                // Add to the database
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Products (ProductId, ProductName, Quantity, Price, CategoryId) VALUES (@ProductId, @ProductName, @Quantity, @Price, @CategoryId)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductId", newProduct.ProductId);
                    command.Parameters.AddWithValue("@ProductName", newProduct.ProductName);
                    command.Parameters.AddWithValue("@Quantity", newProduct.Quantity);
                    command.Parameters.AddWithValue("@Price", newProduct.Price);
                    command.Parameters.AddWithValue("@CategoryId", newProduct.CategoryId);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = productDataGrid.SelectedItem as Product;
            if (selectedProduct != null)
            {
                var inputDialog = new ProductInputDialog
                {
                    ProductId = selectedProduct.ProductId,
                    ProductNameTextBox = { Text = selectedProduct.ProductName },
                    QuantityTextBox = { Text = selectedProduct.Quantity.ToString() },
                    PriceTextBox = { Text = selectedProduct.Price.ToString() },
                    CategoryIdTextBox = { Text = selectedProduct.CategoryId.ToString() }
                };

                if (inputDialog.ShowDialog() == true)
                {
                    if (int.TryParse(inputDialog.QuantityTextBox.Text, out int quantity) &&
                        int.TryParse(inputDialog.PriceTextBox.Text, out int price) &&
                        int.TryParse(inputDialog.CategoryIdTextBox.Text, out int categoryId))
                    {
                        selectedProduct.ProductId = inputDialog.ProductId;
                        selectedProduct.ProductName = inputDialog.ProductNameTextBox.Text;
                        selectedProduct.Quantity = quantity;
                        selectedProduct.Price = price;
                        selectedProduct.CategoryId = categoryId;

                        using (var connection = new SqlConnection(connectionString))
                        {
                            string query = "UPDATE Products SET ProductName = @ProductName, Quantity = @Quantity, Price = @Price, CategoryId = @CategoryId WHERE ProductId = @ProductId";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@ProductName", selectedProduct.ProductName);
                            command.Parameters.AddWithValue("@Quantity", selectedProduct.Quantity);
                            command.Parameters.AddWithValue("@Price", selectedProduct.Price);
                            command.Parameters.AddWithValue("@CategoryId", selectedProduct.CategoryId);
                            command.Parameters.AddWithValue("@ProductId", selectedProduct.ProductId);

                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                MessageBox.Show("Product updated successfully.");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error: {ex.Message}");
                            }
                        }

                        productDataGrid.Items.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid numeric values for Quantity, Price, and Category ID.");
                    }
                }
            }
            else
            {
                MessageBox.Show("No product selected to update.");
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = productDataGrid.SelectedItem as Product;
            if (selectedProduct != null)
            {
                Products.Remove(selectedProduct);

                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Products WHERE ProductId = @ProductId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ProductId", selectedProduct.ProductId);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }
    }
}
