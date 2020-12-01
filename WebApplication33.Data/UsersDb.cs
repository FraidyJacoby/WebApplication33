using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace WebApplication33.Data
{
    public class UsersDb
    {
        private string _connectionString;

        public UsersDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Ad> GetAds()
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Ads";
                conn.Open();
                var ads = new List<Ad>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ads.Add(new Ad
                    {
                        Id = (int)reader["Id"],
                        UserId = (int)reader["UserId"],
                        Title = (string)reader["Title"],
                        Description = (string)reader["Description"],
                        PhoneNumber = (Int64)reader["PhoneNumber"]
                    });
                }
                return ads;
            }
        }

        public void AddAd(Ad ad)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Ads (UserId, Title, Description, PhoneNumber)
                                    VALUES(@userId, @title, @description, @phoneNumber) 
                                    SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@userId", ad.UserId);
                cmd.Parameters.AddWithValue("@title", ad.Title);
                cmd.Parameters.AddWithValue("@description", ad.Description);
                cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
                conn.Open();
                ad.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void DeleteAd(int id)
        {
            using(var conn = new SqlConnection(_connectionString))
            using(var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"DELETE FROM Ads WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public User LogIn(string email, string password)
        {
            var user = GetByEmail(email);
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }
            
            return user;
        }

        public List<Ad> GetAdsById(int userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Ads WHERE UserId = @userId";
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                var reader = cmd.ExecuteReader();
                var ads = new List<Ad>();
                while (reader.Read())
                {
                    ads.Add(new Ad
                    {
                        Id = (int)reader["Id"],
                        UserId = (int)reader["UserId"],
                        Title = (string)reader["Title"],
                        Description = (string)reader["Description"],
                        PhoneNumber = (Int64)reader["PhoneNumber"]
                    });
                }
                return ads;
            }
        }

        public void SignUp(User user, string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            using(var conn = new SqlConnection(_connectionString))
            using(var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Users(Name, Email, PasswordHash)
                                    VALUES(@name, @email, @passwordHash)
                                    SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@passwordHash", hash);
                conn.Open();
                user.Id = (int)(decimal)cmd.ExecuteNonQuery();
            }
        }

        public User GetByEmail(string email)
        {
            using(var conn = new SqlConnection(_connectionString))
            using(var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Users WHERE Email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new User
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Email = (string)reader["Email"],
                    PasswordHash = (string)reader["PasswordHash"]
                };
            }
        }
    }
}
