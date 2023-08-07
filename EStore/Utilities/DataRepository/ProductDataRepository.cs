using EStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
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
            try
            {
                Product findProduct = _dbContext.ProductModel.FirstOrDefault(p => p.Name.Equals(product.Name));
                if (findProduct == null)
                {
                    // Image Conversion to binary data
                    if (product.ImageFile != null && product.ImageFile.ContentLength > 0)
                    {
                        using (var binaryReader = new BinaryReader(product.ImageFile.InputStream))
                        {
                            product.ImageData = binaryReader.ReadBytes(product.ImageFile.ContentLength);
                        }
                    }
                    product.timestamp = DateTime.Now.Date;
                    _dbContext.ProductModel.Add(product);
                    if (_dbContext.SaveChanges() > 0)
                        return true;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return false;
        }

        public bool EditProduct(Product product)
        {
            try
            {
                var findProduct = _dbContext.ProductModel.FirstOrDefault(p => p.Name.Equals(product.Name));
                if (findProduct != null)
                {
                    // Image Conversion to binary data
                    if (product.ImageFile != null && product.ImageFile.ContentLength > 0)
                    {
                        using (var binaryReader = new BinaryReader(product.ImageFile.InputStream))
                        {
                            product.ImageData = binaryReader.ReadBytes(product.ImageFile.ContentLength);
                            findProduct.ImageData = product.ImageData;
                        }
                    }
                    findProduct.Description = product.Description;
                    findProduct.Category = product.Category;
                    findProduct.Cost = product.Cost;
                    _dbContext.SaveChanges();
                    return true;
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        // Log or handle the validation error details
                        System.Diagnostics.Debug.WriteLine($"Entity: {validationErrors.Entry.Entity.GetType().Name}, Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                    }
                }
            }

            return false;
        }

        public Product GetProduct(string productName)
        {
            try
            {
                var findProduct = _dbContext.ProductModel.FirstOrDefault(p => p.Name.Equals(productName));
                if (findProduct != null)
                {
                    findProduct.ImageSrc = GetBase64ImageSrc(findProduct.ImageData);
                    return findProduct;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return new Product();
        }

        public List<TotalSales> GetImagesForTotalSales(List<TotalSales> salesItems)
        {
            for(int i = 0; i < salesItems.Count; i++)
            {
                string itemName = salesItems[i].ProductName;
                try
                {
                    Product product = _dbContext.ProductModel.FirstOrDefault(p => p.Name.Equals(itemName));

                    if (product != null)
                    {
                        salesItems[i].ImageSrc = GetBase64ImageSrc(product.ImageData);
                    }
                } 
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }

            return salesItems;
        }

        public List<Product> GetAllProducts()
        {
            try
            {
                var products = _dbContext.ProductModel.ToList();
                if (products != null)
                {
                    foreach (var product in products)
                    {
                        product.ImageSrc = GetBase64ImageSrc(product.ImageData);
                    }

                    return products;
                }
            } 
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
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
            try
            {
                var productCategories = _dbContext.ProductCategoriesModel.ToList();
                if (productCategories != null)
                {
                    List<string> categories = new List<string>();
                    foreach (var category in productCategories)
                    {
                        categories.Add(category.Type);
                    }

                    return categories;
                }
            } 
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return new List<string>();
        }

        public void DeleteProduct(string productId)
        {
            try
            {
                var findProduct = _dbContext.ProductModel.FirstOrDefault(p => p.Name.Equals(productId));
                if (findProduct != null)
                {
                    _dbContext.ProductModel.Remove(findProduct);
                    _dbContext.SaveChanges();
                }
            } 
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public List<TotalSales> MapProductToCategory(List<TotalSales> saleItems)
        {
            var products = GetAllProducts();
            foreach (var saleItem in saleItems)
            {
                var product = products.FirstOrDefault(p => p.Name == saleItem.ProductName);
                if(product != null)
                {
                    saleItem.Category = product.Category;
                }
            }
            return saleItems;
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
                        cmd.Parameters.Add("@" + VariableNameHelper.GetPropertyName(() => cart.PaymentType), SqlDbType.VarChar)
                            .Value = cart.PaymentType;
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