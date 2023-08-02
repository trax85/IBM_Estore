using EStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EStore.Utilities.DataRepository
{
    public class ProductDataRepository :Connection
    {
        public bool createProduct(Product product)
        {
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("createProduct", _connection))
            {
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.Name), SqlDbType.VarChar)
                    .Value = product.Name;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.Category), SqlDbType.VarChar)
                    .Value = product.Category;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.Description), SqlDbType.VarChar)
                    .Value = product.Description;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.AdditionalDescription),
                    SqlDbType.VarChar).Value = product.AdditionalDescription;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.Cost), SqlDbType.Int)
                    .Value = product.Cost;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return false;
         }

        public bool editProduct(Product product)
        {
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("updateProduct", _connection))
            {
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.Name), SqlDbType.VarChar)
                    .Value = product.Name;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.Category), SqlDbType.VarChar)
                    .Value = product.Category;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.Description), SqlDbType.VarChar)
                    .Value = product.Description;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.AdditionalDescription), SqlDbType.VarChar)
                    .Value = product.AdditionalDescription;
                cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.Cost), SqlDbType.Int)
                    .Value = product.Cost;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return false;
        }

        public Product getProduct(string productName)
        {
            Product product = new Product();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("getProductByName", _connection))
            {
                try
                {
                    cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => product.Name), SqlDbType.VarChar)
                        .Value = productName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    _connection.Open();
                    ModelReader productModelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        product = productModelReader.getProduct(reader);
                    }
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return product;
        }
        public List<Product> getAllProducts()
        {
            List<Product> productList = new List<Product>();
            string query = "SELECT * FROM products;";
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                try
                {
                    _connection.Open();
                    ModelReader productModelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        productList = productModelReader.getProductList(reader);
                    }
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            return productList;
        }

        public void orderProduct(List<Cart> cartItems, string userName)
        {
            using (_connection = new SqlConnection(_sqlConnectionString))
            {
                foreach (var item in cartItems)
                {
                    using (SqlCommand cmd = new SqlCommand("orderProduct", _connection))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = userName;
                            cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => item.Name), SqlDbType.VarChar)
                                .Value = item.Name;
                            cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => item.Quantity), SqlDbType.VarChar)
                                .Value = item.Quantity;
                            cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => item.Cost), SqlDbType.VarChar)
                                .Value = item.Cost;
                            _connection.Open();
                            cmd.ExecuteNonQuery();
                            _connection.Close();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Write(ex.Message);
                        }
                    }
                }
            }
        }

        public List<string> getProductCategories()
        { 
            List<string> categories = new List<string>();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("getProductCategories", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    _connection.Open();
                    ModelReader modelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        categories = modelReader.GetProductCategories(reader);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return categories;
        }

        public void deleteProduct(string productId)
        {
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("deleteProduct", _connection))
            {
                cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = productId;
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}