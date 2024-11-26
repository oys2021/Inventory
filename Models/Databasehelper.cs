using System;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;



public class Databasehelper{

private readonly string _connectionstring;
private readonly IConfiguration _configuration;

public Databasehelper(string connectionstring,IConfiguration configuration){
    _connectionstring=connectionstring;
    _configuration=configuration;
}



public void SendPasswordToUser(string userEmail, string password,string username)
{
    try
    {
        // Create the email message
        var fromAddress = new MailAddress("yawsarfo2019@domain.com", "ISMS");
        var toAddress = new MailAddress(userEmail);
        string subject = "Your Login Credentials";
        string body = $@"
            Hello,

            Welcome to [ISMS].

            Here are your login credentials:

            Username: {username}
            Password: {password}

            Please log in and change your password immediately.

            Best regards,
            [Your Company Name]
        ";

        // SMTP client configuration
        using (var smtp = new SmtpClient())
        {
            smtp.Host = "smtp.yourprovider.com";  // Use SMTP server of your provider
            smtp.Port = 587;  // Or your provider's SMTP port
            smtp.Credentials = new NetworkCredential("yawsarfo@gmail.com", "yourpassword");
            smtp.EnableSsl = true;

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            // Send email
            smtp.Send(message);
        }

        Console.WriteLine("Password sent successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending email: {ex.Message}");
    }
}





public List<User> getUser(string username,string ? email)
{
    var newlist = new List<User>();
    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = "SELECT * FROM Users WHERE Username = @Username OR Email=@Email";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@Email", email);

        connection.Open();
        using (var result = command.ExecuteReader())
        {
            Console.WriteLine("Query executed. Inspecting results...");

            while (result.Read()) 
            {
                Console.WriteLine("Row data:");
                var user = new User
                {
                    id = result["UserId"] != DBNull.Value ? Convert.ToInt32(result["UserId"]) : 0,
                    username = result["Username"].ToString(),
                    email = result["Email"].ToString(),
                    password = result["PasswordHash"].ToString(),
                    role = result["RoleId"] != DBNull.Value ? Convert.ToInt32(result["RoleId"]) : 0,
                    isActive = result["IsActive"] != DBNull.Value && Convert.ToBoolean(result["IsActive"]),
                    firstname = result["firstname"].ToString(),
                    lastname = result["lastname"].ToString(),
                    phone = result["phone"].ToString(),
                    CreatedAt = result["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(result["CreatedAt"]) : DateTime.MinValue,
                    RequirePasswordChange = result["RequirePasswordChange"] != DBNull.Value && Convert.ToBoolean(result["RequirePasswordChange"]),

                };

                newlist.Add(user);
            }
        }
    }

    return newlist.Count > 0 ? newlist : null;
}
public List<User> getUserbyUsername(string username)
{
    var newlist = new List<User>();
    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = "SELECT * FROM Users WHERE Username = @Username";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Username", username);

        connection.Open();
        using (var result = command.ExecuteReader())
        {
            Console.WriteLine("Query executed. Inspecting results...");

            while (result.Read()) 
            {
                Console.WriteLine("Row data:");
                var user = new User
                {
                    id = result["UserId"] != DBNull.Value ? Convert.ToInt32(result["UserId"]) : 0,
                    username = result["Username"].ToString(),
                    email = result["Email"].ToString(),
                    password = result["PasswordHash"].ToString(),
                    role = result["RoleId"] != DBNull.Value ? Convert.ToInt32(result["RoleId"]) : 0,
                    isActive = result["IsActive"] != DBNull.Value && Convert.ToBoolean(result["IsActive"]),
                    firstname = result["firstname"].ToString(),
                    lastname = result["lastname"].ToString(),
                    phone = result["phone"].ToString(),
                    CreatedAt = result["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(result["CreatedAt"]) : DateTime.MinValue,
                    RequirePasswordChange = result["RequirePasswordChange"] != DBNull.Value && Convert.ToBoolean(result["RequirePasswordChange"]),

                };

                newlist.Add(user);
            }
        }
    }

    return newlist.Count > 0 ? newlist : null;
}

public bool Userexist(string username, string password){
    using(var connection=new SqlConnection(_connectionstring)){
        var query="SELECT * FROM Users WHERE username=@Username OR password=@PasswordHash";
        var command=new SqlCommand(query,connection);
        command.Parameters.AddWithValue("@Username",username);
        command.Parameters.AddWithValue("@PasswordHash",HashPassword(password));

        connection.Open();
        var result=command.ExecuteReader();

        if (result.Read()){
            return true;
        }
    }
    return false;
}




public bool createuser(string username, string email, int role, string firstname, string lastname, string phone)
{
    string password = GenerateRandomPassword(12);  // Generate the password
    Console.WriteLine("Generated Password: " + password);
    
    // Insert user into the database
    using (var connection = new SqlConnection(_connectionstring))
    {
        var query = "INSERT INTO Users (Username, Email, PasswordHash, RoleId, IsActive, firstname, lastname, phone) " +
                    "VALUES(@Username, @Email, @PasswordHash, @RoleId, @IsActive, @firstname, @lastname, @phone)";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@PasswordHash", HashPassword(password)); // Store hashed password
        command.Parameters.AddWithValue("@RoleId", role);
        command.Parameters.AddWithValue("@IsActive", 1);
        command.Parameters.AddWithValue("@firstname", firstname);
        command.Parameters.AddWithValue("@lastname", lastname);
        command.Parameters.AddWithValue("@phone", phone);

        connection.Open();
        var result = command.ExecuteNonQuery();

        // 
        if (result > 0)
        {
            var emailHelper = new EmailHelper(_configuration);
            emailHelper.SendEmail(email, "Your Account Details", 
                $"Hello {firstname} {lastname},\n\nYour account has been created successfully.\n" +
                $"Your temporary password is: {password}\n\nPlease log in and change your password as soon as possible.");
            Console.WriteLine("Sent");
            return true;
        }
        else
        {
            return false;
        }
    }
}

public bool createrole(string rolename,string description){
    using(var connection=new SqlConnection(_connectionstring)){
        var query="INSERT INTO Roles(rolename,role_description) VALUES (@rolename,@role_description)";
        var command=new SqlCommand(query,connection);
        command.Parameters.AddWithValue("@rolename",rolename);
        command.Parameters.AddWithValue("@role_description",description);

        connection.Open();
        var result=command.ExecuteNonQuery();
        return result>0;
        }
}

public bool UpdatePassword(int userId, string hashedPassword)
{
    try
    {
        // Use the 'using' block to manage resources
        using (var connection = new SqlConnection(_connectionstring))
        {
            var sqlQuery = "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserId = @UserId";
            var command = new SqlCommand(sqlQuery, connection);
            
            command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
            command.Parameters.AddWithValue("@UserId", userId);

            // Open the connection if it's not already open
            connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"SQL Exception: {ex.Message}");
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        return false; 
    }
}

  
public List<Role> getRole(){
    var newrole=new List<Role>();
    using (var connection=new SqlConnection(_connectionstring)){
        var query="SELECT * FROM Roles";
        var command=new SqlCommand(query,connection);
        connection.Open();

        var result=command.ExecuteReader();

        while (result.Read()){
            var role= new Role{
            id=result.GetInt32(0),
            rolename=result.GetString(1),
            description=result.GetString(2),
        };
        newrole.Add(role);
        }
    }
    return newrole;
}


 // Method to generate a random password
    private string GenerateRandomPassword(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=<>?";
        char[] password = new char[length];

        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            for (int i = 0; i < length; i++)
            {
                int randomIndex = randomBytes[i] % validChars.Length;
                password[i] = validChars[randomIndex];
            }
        }

        return new string(password);
    }

 public string HashPassword(string password){
        using(var sha256 =SHA256.Create()){

            var bytes=Encoding.UTF8.GetBytes(password);
            var hash=sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        
    }
}