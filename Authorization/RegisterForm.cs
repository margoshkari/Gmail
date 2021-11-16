using Client;
using System;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class RegisterForm : Form
    {
       
        public RegisterForm()
        {
          
            InitializeComponent();
        }

        private void RegisterForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            ClientData.socket.Send(Encoding.Unicode.GetBytes("login"));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != string.Empty && this.textBox2.Text != string.Empty && this.textBox3.Text != string.Empty)
            {
                if(this.textBox2.Text.Length < 5)
                    MessageBox.Show("Пароль должен быть более 5 символов!");
                else if(!IsValidEmail(this.textBox3.Text))
                    MessageBox.Show("Неверный формат почты!");
                else
                {
                    ClientData.socket.Send(Encoding.Unicode.GetBytes("register"));
                    ClientData.GetMsg();
                    ClientData.socket.Send(Encoding.Unicode.GetBytes($"{this.textBox3.Text}:{this.textBox1.Text}:{this.textBox2.Text}"));

                    string msg = ClientData.GetMsg();
                    if (msg == "success")
                    {
                        this.Close();
                    }
                    else if (msg == "exist")
                    {
                        MessageBox.Show("Такая почта уже существует!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля!");
            }
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            ClientData.socket.Send(Encoding.Unicode.GetBytes("login"));
            this.Close();
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
