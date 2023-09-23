using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using ProductsWpfUsingCrud.Entity;

namespace ProductsWpfUsingCrud
{
    public partial class MainWindow : Window
    {
        string connectionString = "Server=DESKTOP-5QV11V1;Database=Productdb;Integrated Security=True;";

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            RefreshDataGrid(); 
        }

        private void RefreshDataGrid()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

        
                string selectQuery = "SELECT * FROM ProductsDetails";
                using (SqlCommand cmd = new SqlCommand(selectQuery, connection))
                {
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    List<ProductDetails> productList = new List<ProductDetails>();

                    while (dataReader.Read())
                    {
                        ProductDetails product = new ProductDetails
                        {
                            ProductId = dataReader.GetInt32(0),           // Assuming ProductId is the first column (index 0)
                            Name = dataReader.GetString(1),              
                            Description = dataReader.GetString(2)        
                        };

                        productList.Add(product);
                    }

                    dataGrid.ItemsSource = productList;

                }
            }
        }

        private void insert_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string name = txtName.Text;
                string description = txtDescription.Text;

               

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
                {
                    MessageBox.Show("Please enter values for Name and Description.", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    string insertQuery = "INSERT INTO ProductsDetails (Name, Description) VALUES (@Name, @Description)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Description", description);

                        cmd.ExecuteNonQuery();
                    }
                }
                RefreshDataGrid(); 
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            string id = txtID.Text;
            string name = txtName.Text;
            string description = txtDescription.Text;

            if (!string.IsNullOrEmpty(id))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
                    {
                        MessageBox.Show("Please enter values for Name and Description.", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    string updateQuery = "UPDATE ProductsDetails SET Name = @Name, Description = @Description WHERE ProductId = @ID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Description", description);

                        cmd.ExecuteNonQuery();
                    }

                    RefreshDataGrid();
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string id = txtID.Text;

            if (!string.IsNullOrEmpty(id))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM ProductsDetails WHERE ProductId = @ID";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);

                        cmd.ExecuteNonQuery();
                    }

                    RefreshDataGrid();
                }
            }
        }


        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // Get the selected item from the DataGrid
            if (dataGrid.SelectedItem is ProductDetails selectedProduct)
            {
                // Fill the text boxes with the selected product's data
                txtID.Text = selectedProduct.ProductId.ToString();
                txtName.Text = selectedProduct.Name;
                txtDescription.Text = selectedProduct.Description;
            }
        }



    }
}
