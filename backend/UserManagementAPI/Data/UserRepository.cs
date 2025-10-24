using System.Data;
using Microsoft.Data.SqlClient;
using UserManagementAPI.Models;
namespace UserManagementAPI.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "SELECT Id, Name, DateOfBirth, Designation, Email, PasswordHash FROM Users";
            using SqlCommand cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DateOfBirth = reader.GetDateTime(2),
                    Designation = reader.GetString(3),
                    Email = reader.GetString(4),
                    PasswordHash = reader.GetString(5)
                });
            }
            return users;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "SELECT Id, Name, DateOfBirth, Designation, Email, PasswordHash FROM Users WHERE Email=@Email";
            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DateOfBirth = reader.GetDateTime(2),
                    Designation = reader.GetString(3),
                    Email = reader.GetString(4),
                    PasswordHash = reader.GetString(5)
                };
            }
            return null;
        }

        public async Task AddUserAsync(User user)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "INSERT INTO Users (Name, DateOfBirth, Designation, Email, PasswordHash) VALUES (@Name, @DOB, @Designation, @Email, @PasswordHash)";
            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@DOB", user.DateOfBirth);
            cmd.Parameters.AddWithValue("@Designation", user.Designation);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "UPDATE Users SET Name=@Name, DateOfBirth=@DOB, Designation=@Designation, PasswordHash=@PasswordHash WHERE Email=@Email";
            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@DOB", user.DateOfBirth);
            cmd.Parameters.AddWithValue("@Designation", user.Designation);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteUserAsync(string email)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "DELETE FROM Users WHERE Email=@Email";
            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}