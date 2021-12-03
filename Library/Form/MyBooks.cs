using Library.Class;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Library
{
    public partial class MyBooks : Form
    {
        Reader reader;

        List<Book> books;

        public MyBooks(Reader reader)
        {
            InitializeComponent();
            UpBooksAsync();
            this.reader = reader;
        }

        private void MyBooks_Load(object sender, EventArgs e)
        {
            UpList();
        }

        private void UpList() 
        {
            listBox1.Items.AddRange(reader.books.ToArray());
        }

        private async void UpBooksAsync() 
        {
            await using (antContext db = new antContext()) 
            {
                books = db.Books.ToList();
            }
        }

        private Book FindBook(string title)
        {
            foreach (var item in books)
            {
                if (item.title == title)
                {
                    return item;
                }
            }

            return null;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Book book = FindBook(listBox1.SelectedItem?.ToString());
            if (book == null) return;

            pictureBox3.Image = Image.FromFile(book.img);
            richTextBox3.Text = book.desc;
            textBox3.Text = book.title;
        }

        private void MyBooks_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason.ToString() == "UserClosing")
            {
                e.Cancel = true;
            }
            Hide();
        }

        private void MyBooks_SizeChanged(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }
    }
}
