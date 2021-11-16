using Authorization;
using Client;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Login : Form
    {
      
        public User user = new User();
        public string json = string.Empty;
        private bool isLogin = false;
        public Login()
        {
            ClientData.socket.Connect(ClientData.iPEndPoint);
            InitializeComponent();
            if (!Directory.Exists(@"C:\ProgramData\RegisterForm"))
            {
                Directory.CreateDirectory(@"C:\ProgramData\RegisterForm");
            }
        }

        private void Login_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (!isLogin)
                Environment.Exit(0);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != string.Empty && this.textBox2.Text != string.Empty)
            {
                ClientData.socket.Send(Encoding.Unicode.GetBytes("login"));
                ClientData.GetMsg();
                ClientData.socket.Send(Encoding.Unicode.GetBytes($"{this.textBox1.Text}:{this.textBox2.Text}"));

                string msg = ClientData.GetMsg();
                if (msg == "success")
                {
                    Main.user = JsonSerializer.Deserialize<User>(File.ReadAllText(@$"C:\ProgramData\RegisterForm\{this.textBox1.Text}.json"));
                    File.WriteAllText("UserLogin.txt", this.textBox1.Text);
                    ClientData.socket.Send(Encoding.Unicode.GetBytes("gmail"));
                    GmailForm gmail = new GmailForm();
                    this.Hide();
                    gmail.ShowDialog();
                    this.Show();
                }
                else if (msg == "fail")
                {
                    MessageBox.Show("Неправильный пароль!");
                }
                else
                {
                    MessageBox.Show("Такого пользователя не существует!\nСначала зарегестрируйтесь!");
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClientData.socket.Send(Encoding.Unicode.GetBytes("register"));
            RegisterForm register = new RegisterForm();
            this.Hide();
            register.ShowDialog();
            this.Show();
        }
    }
}
