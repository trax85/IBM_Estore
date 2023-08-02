﻿using EStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EStore.Utilities.DataRepository
{
    public class CartDataRepository : Connection
    {
        public List<Cart> getPurchaseHistory(string userName)
        {
            List<Cart> cartList = new List<Cart>();
            using (_connection = new SqlConnection(_sqlConnectionString))
            using (SqlCommand cmd = new SqlCommand("getPurchaseHistoryByUsername", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = userName;
                try
                {
                    _connection.Open();
                    ModelReader cartModelReader = new ModelReader();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        cartList = cartModelReader.getCartList(reader);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return cartList;
        }
    }
}