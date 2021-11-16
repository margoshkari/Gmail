using MailKit;
using MailKit.Net.Imap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp1;
namespace Authorization
{
    public partial class GmailForm : Form
    {
        ImapClient client = new ImapClient();
        IMailFolder inbox;
        string search = string.Empty;
        List<string> allMsg = new List<string>();
        User user = new User();
        List<string> date = new List<string>();
        public GmailForm()
        {
            user = JsonSerializer.Deserialize<User>(File.ReadAllText(@$"C:\ProgramData\RegisterForm\{File.ReadAllText("UserLogin.txt")}.json"));
            InitializeComponent();
            DownloadMessages();
            
            ListMsg();
        }
        public void DownloadMessages()
        {

            client.Connect("imap.gmail.com", 993, true);
            client.Authenticate(user.Email, user.Password);
            inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);
            for (int i = 0; i < inbox.Count; i++)
            {
                allMsg.Add(inbox.GetMessage(i).From.ToString());
                date.Add(inbox.GetMessage(i).Date.ToString());
            }
        }
        public void ListMsg()
        {
            this.listBox1.Items.Clear();
            for (int i = 0; i < inbox.Count; i++)
            {
                if (search == string.Empty)
                    this.listBox1.Items.Add(inbox.GetMessage(i).From);
                else if (inbox.GetMessage(i).From.ToString().Contains(search))
                    this.listBox1.Items.Add(inbox.GetMessage(i).From);
                else if (inbox.GetMessage(i).Date.ToString().Contains(search))
                {
                    this.listBox1.Items.Add(inbox.GetMessage(i).From);
                }
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = allMsg.FindIndex(item => item.Contains((sender as ListBox).SelectedItem.ToString()));
            this.textBox1.Text = inbox.GetMessage(index).GetTextBody(MimeKit.Text.TextFormat.Text);
        }
        private void SearchByDate()
        {
            int index = date.FindIndex(item => item.Contains(search));
            ListMsg();
            this.textBox1.Text = inbox.GetMessage(index).TextBody;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            search = this.textBox2.Text;
            ListMsg();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            search = this.textBox2.Text;
            if (search == string.Empty)
                ListMsg();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime dateTime = dateTimePicker1.Value;
            search = $"{dateTime.Day}.{dateTime.Month}.{dateTime.Year}";
            SearchByDate();
        }
    }
}
