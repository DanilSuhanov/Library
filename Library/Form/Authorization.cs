using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library.Class;

namespace Library
{
    public partial class Form1 : Form
    {
        private bool dateIsUp = false;

        List<Reader> readers;

        Reader reader;

        private bool isNew = true;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(bool isNew)
        {
            InitializeComponent();
            this.isNew = isNew;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!dateIsUp)
            {
                MessageBox.Show("Данные ещё не загружены");
                return;
            }
            try
            {
                if (!AvailabilityСheckReader(textBox1.Text))
                {
                    Reader reader = new(textBox1.Text, textBox2.Text, Convert.ToUInt32(textBox3.Text));
                    AddReader(reader);

                    MessageBox.Show("Пользователь " + reader.Name + " создан!");
                }
                else
                {
                    MessageBox.Show("Такой пользователь уже существует");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка ввода");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AuthUser(textBox5.Text, textBox4.Text);
        }

        private void AuthUser(string login, string password)
        {
            if (!dateIsUp)
            {
                MessageBox.Show("Данные ещё не загружены");
                return;
            }
            try
            {
                if (UserVerification(login, password))
                {
                    Session.main = new(reader);
                    Hide();
                    Session.main.Show();
                    WriteToTemp(reader);
                }
                else
                {
                    MessageBox.Show("Неверное имя или пароль");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка ввода");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!dateIsUp)
            {
                MessageBox.Show("Данные ещё не загружены!");
                return;
            }
            if (UserVerification(textBox5.Text, textBox4.Text))
            {
                if (reader.IsAdmin)
                {
                    this.Hide();
                    new MainModer().Show();
                }
            }
        }

        private async void WriteToTemp(Reader reader)
        {
            await Task.Run(() => 
            {
                using (FileStream fs = new FileStream("AuthTemp.txt", FileMode.OpenOrCreate))
                {
                    byte[] mes = System.Text.Encoding.Default.GetBytes($"{JsonSerializer.Serialize<Reader>(reader)}");
                    fs.Write(mes, 0, mes.Length);
                }
            });
        }

        private async void AsyncUp()
        {
            dateIsUp = false;
            await Task.Run(() =>
            {
                using (antContext db = new())
                {
                    readers = db.Readers.ToList();
                }

                dateIsUp = true;
            });
        }

        private void AddReader(Reader reader)
        {
            this.reader = reader;
            using (antContext db = new())
            {
                db.Readers.Add(reader);
                db.SaveChanges();
            }
        }

        private bool AvailabilityСheckReader(string ReaderName)
        {
            foreach (var item in readers) if (item.Name == ReaderName) return true;
            return false;
        }

        private bool UserVerification(string name, string password)
        {
            foreach (var item in readers)
            {
                if (item.Name == name)
                {
                    if (item.password == password)
                    {
                        reader = item;
                        return true;
                    }
                }
            }

            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AsyncUp();
            if(isNew) AutoAuth();
        }

        private async void AutoAuth() 
        {
            Reader reader1 = null;
            await Task.Run(() => 
            {
                while (!dateIsUp) Thread.Sleep(100);
                reader1 = GetTempAuth();
            });
            if (reader1 == null) return;
            AuthUser(reader1.Name, reader1.password);
        }

        private Reader GetTempAuth() 
        {
            if (!File.Exists("AuthTemp.txt")) return null;

            byte[] mes;

            using (FileStream fs = File.OpenRead("AuthTemp.txt"))
            {
                mes = new byte[fs.Length];
                fs.Read(mes, 0, mes.Length);
            }

            return JsonSerializer.Deserialize<Reader>(System.Text.Encoding.Default.GetString(mes));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }
    }
}