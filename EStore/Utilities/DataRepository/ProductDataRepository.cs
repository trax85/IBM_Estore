using EStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.WebPages;

namespace EStore.Utilities.DataRepository
{
    public class ProductDataRepository : SqlDbConnection, IProductDataRepository 
    {
        public bool CreateProduct(Product product)
        {
            Product findProduct = _dbContext.ProductModel.FirstOrDefault(p => p.Name.Equals(product.Name));
            if (findProduct == null)
            {
                // Image Conversion to binary data
                if(product.ImageFile != null && product.ImageFile.ContentLength > 0)
                {
                    using(var binaryReader = new BinaryReader(product.ImageFile.InputStream))
                    {
                        product.ImageData = binaryReader.ReadBytes(product.ImageFile.ContentLength);
                    }
                }
                product.timestamp = DateTime.Now.Date;
                _dbContext.ProductModel.Add(product);
                if (_dbContext.SaveChanges() > 0)
                    return true;
            }

            return false;
        }

        public bool EditProduct(Product product)
        {
            var findProduct = _dbContext.ProductModel.FirstOrDefault(p => p.Name.Equals(product.Name));
            if(findProduct != null)
            {
                // Image Conversion to binary data
                if (product.ImageFile != null && product.ImageFile.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(product.ImageFile.InputStream))
                    {
                        product.ImageData = binaryReader.ReadBytes(product.ImageFile.ContentLength);
                    }
                }
                findProduct.ImageData = product.ImageData;
                findProduct.Description = product.Description;
                findProduct.Category = product.Category;
                findProduct.Cost = product.Cost;
                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public Product GetProduct(string productName)
        {
            var findProduct = _dbContext.ProductModel.FirstOrDefault(p => p.Name.Equals(productName));
            if(findProduct != null )
            {
                findProduct.ImageSrc = GetBase64ImageSrc(findProduct.ImageData);
                return findProduct;
            }
            return new Product();
        }

        public List<Cart> GetImagesForCart(List<Cart> cartItems)
        {
            for(int i = 0; i < cartItems.Count; i++)
            {
                string itemName = cartItems[i].Name;
                Product product = _dbContext.ProductModel.FirstOrDefault( p => p.Name.Equals(itemName));
                
                if (product != null)
                {
                    cartItems[i].ImageSrc = GetBase64ImageSrc(product.ImageData);
                }
            }

            return cartItems;
        }

        public List<Product> GetAllProducts()
        {
            var products = _dbContext.ProductModel.ToList();
            if(products != null)
            {
                foreach(var product in products)
                {
                    product.ImageSrc = GetBase64ImageSrc(product.ImageData);
                }

                return products;
            }

            return new List<Product>();
        }

        private string GetBase64ImageSrc(byte[] imageData)
        {
            if (imageData != null && imageData.Length > 0)
            {
                string base64Image = Convert.ToBase64String(imageData);
                return $"data:image/png;base64,{base64Image}";
            }

            // Return a placeholder image URL or any default image
            return "https://dummyimage.com/450x300/dee2e6/6c757d.jpg";
        }

        public List<string> GetProductCategories()
        {
            var productCategories = _dbContext.ProductCategoriesModel.ToList();
            if(productCategories != null)
            {
                List<string> categories = new List<string>();
                foreach (var category in productCategories)
                {
                    categories.Add(category.Type);
                }

                return categories;
            }

            return new List<string>();
        }

        public void DeleteProduct(string productId)
        {
            var findProduct = _dbContext.ProductModel.FirstOrDefault(p => p.Name.Equals(productId));
            if(findProduct != null)
            {
                _dbContext.ProductModel.Remove(findProduct);
                _dbContext.SaveChanges();
            }
        }
    }

    public class ProductDataRepositoryV2 : Connection, IProductDataRepositoryV2
    {
        public bool OrderProduct(Cart cart, string userName)
        {
            using (_connection = new SqlConnection(_sqlConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("orderProduct", _connection))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = userName;
                        cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => cart.Name), SqlDbType.VarChar)
                            .Value = cart.Name;
                        cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => cart.Quantity), SqlDbType.VarChar)
                            .Value = cart.Quantity;
                        cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => cart.Cost), SqlDbType.VarChar)
                            .Value = cart.Cost;
                        _connection.Open();
                        cmd.ExecuteNonQuery();
                        _connection.Close();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write(ex.Message);
                        _connection.Close();
                    }
                }
            }
            return true;
        }
    }
}