using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using MimeKit;
using MailKit.Net.Smtp;

namespace Server
{
    class Program
    {
        static ServerData serverData = new ServerData();
        static List<User> users = new List<User>();
        static User user = new User();
        static void Main(string[] args)
        {
            Console.WriteLine("Start server...");
            try
            {
                serverData.socket.Bind(serverData.iPEndPoint);
                serverData.socket.Listen(10);

                Task.Factory.StartNew(() => Connect());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
        static void Connect()
        {
            while (true)
            {
                serverData.socketClient = serverData.socket.Accept();
                serverData.socketClientsList.Add(serverData.socketClient);

                Task.Factory.StartNew(() => Authorization());
            }
        }
        static void Authorization()
        {
            Console.WriteLine("Authorization");
            bool isLogin = false;
            string msg = string.Empty;
            while (!isLogin)
            {
                msg = serverData.GetMsg();
                if (msg == "login")
                {
                    serverData.socketClient.Send(Encoding.Unicode.GetBytes("login"));
                    msg = serverData.GetMsg();
                    if (File.Exists(@$"C:\ProgramData\RegisterForm\{msg.Split(':')[0]}.json"))
                    {
                        user = JsonSerializer.Deserialize<User>(File.ReadAllText(@$"C:\ProgramData\RegisterForm\{msg.Split(':')[0]}.json"));
                        if (user.Password ==msg.Split(':')[1])
                        {
                            serverData.socketClient.Send(Encoding.Unicode.GetBytes("success"));
                            isLogin = false;
                            Console.WriteLine($"Client connected!");
                        }
                        else
                        {
                            serverData.socketClient.Send(Encoding.Unicode.GetBytes("fail"));
                        }
                    }
                    else
                    {
                        serverData.socketClient.Send(Encoding.Unicode.GetBytes("not found"));
                    }
                }
                else
                {
                    Registration();
                }
            }
        }
        static void Registration()
        {
            Console.WriteLine("Registration");
            bool isRegister = false;
            string msg = string.Empty;
            while (!isRegister)
            {
                msg = serverData.GetMsg();
                if (msg == "register")
                {
                    serverData.socketClient.Send(Encoding.Unicode.GetBytes("register"));
                    msg = serverData.GetMsg();
                    user.Email = msg.Split(':')[0];
                    user.Login = msg.Split(':')[1];
                    user.Password = msg.Split(':')[2];
                    if (UserExist(user.Email))
                    {
                        serverData.socketClient.Send(Encoding.Unicode.GetBytes("exist"));
                    }
                    else
                    {
                        serverData.socketClient.Send(Encoding.Unicode.GetBytes("success"));
                        isRegister = true;
                        SendPassword().GetAwaiter().GetResult();
                        string json = JsonSerializer.Serialize<User>(user);
                        File.WriteAllText(@$"C:\ProgramData\RegisterForm\{user.Login}.json", json);
                    }
                }
                else if (msg == "login")
                {
                    Authorization();
                }
            }
            Authorization();
        }
        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        static async Task SendPassword()
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "tanksproga@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("Hello", user.Email));
            emailMessage.Subject = "тема";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = ComputeSha256Hash(user.Password)
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("tanksproga@gmail.com", "tanks123");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

        static bool UserExist(string email)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Email == email)
                    return true;
            }
            return false;
        }
    }
}