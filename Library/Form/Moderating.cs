using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library.Class;

namespace Library
{
    public partial class Moderating : Form
    {
        OpenFileDialog open;

        List<Book> books;
        List<Reader> readers;

        List<Reader> readersChangesBook;

        private Book book;

        private bool IsAdd = true;

        public Moderating()
        {
            InitializeComponent();
        }

        private void Moderating_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Moderating_Load(object sender, EventArgs e)
        {
            Up();
            UpList();
        }

        private void UpList()
        {
            listBox1.Items.Clear();
            foreach (var item in books) listBox1.Items.Add(item.title);
        }

        private void UpList(List<Book> books)
        {
            listBox1.Items.Clear();

            foreach (var item in books) listBox1.Items.Add(item.title);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex == -1) return;
                if (listBox1.SelectedItem == null) return;

                book = GetBook(listBox1.SelectedItem.ToString());

                textBox1.Text = book.title;
                richTextBox1.Text = book.desc;
                textBox3.Text = book.img.ToString();
                textBox5.Text = Convert.ToString(book.Quantity);
                listBox2.SelectedItem = book.category;
                checkBox1.Checked = book.forAdults;

                button1.Text = "Изменить";

                IsAdd = false;
            }
            catch { }
        }
        private Book GetBook(string title)
        {
            return books.Find((Book book) => { if (book.title == title) return true; else return false; });
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            richTextBox1.Clear();
            textBox3.Clear();
            textBox5.Clear();
            listBox1.ClearSelected();
            listBox2.ClearSelected();

            IsAdd = true;
            button1.Text = "Добавить";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IsAdd)
            {
                Add();
            }
            else
            {
                DeleteBook(GetBook(listBox1.SelectedItem.ToString()), true);
                AddChangBook(Add());
            }
        }

        private void AddChangBook(Book book)
        {
            try
            {
                foreach (var item in readersChangesBook) item.books.Add(book.title);
            }
            catch { }

            using (antContext db = new antContext())
            {
                db.Readers.UpdateRange(readers);
                db.SaveChanges();
            }
        }

        private void DeleteBook(Book book, bool IsChange)
        {
            using (antContext db = new antContext())
            {
                db.Books.Remove(book);
                db.SaveChanges();
            }
            books.Remove(book);
            DeleteReadersBook(book, IsChange);
        }

        private Book Add() 
        {
            int _quantity;
            try
            {
                _quantity = Convert.ToInt32(textBox5.Text);
            }
            catch
            {
                MessageBox.Show("Ошибка! Введите число в поле количество");
                return null;
            }
            if (AvailabilityСheckBook(textBox1.Text, richTextBox1.Text, checkBox1.Checked, listBox2.SelectedItem.ToString(), textBox3.Text, _quantity))
            {
                MessageBox.Show("Такая книга уже существует");
                return null;
            }
            try
            {
                Book book = new Book
                (
                    title: textBox1.Text,
                    desc: richTextBox1.Text,
                    quantity: _quantity,
                    category: listBox2.SelectedItem.ToString(),
                    forAdults: checkBox1.Checked
                );
                _UnificationId(book);
                if (textBox3.Text != "" && textBox3.Text != "-")
                {
                    book.img = textBox3.Text;
                }
                AddBook(ref book);
                Up();
            }
            catch
            {
                MessageBox.Show("Ошибка ввода!");
                return null;
            }
            if (IsAdd) MessageBox.Show("Книга добавлена!");
            else MessageBox.Show("Книга изменена!");
            UpList();
            return book;
        }

        private void AddBook(ref Book book)
        {
            using (antContext db = new antContext())
            {
                db.Books.Add(book);
                db.SaveChanges();
            }
        }

        private void Up()
        {
            using (antContext db = new antContext())
            {
                books = db.Books.ToList();
                readers = db.Readers.ToList();
            }
        }

        private bool AvailabilityСheckBook(string BookTitle, string BookDesc, bool forAdults, string category, string img, int quantity)
        {
            foreach (var item in books)
            {
                if (item.title == BookTitle && item.desc == BookDesc && item.forAdults == forAdults && item.category == category && item.img == img && item.Quantity == quantity)
                {
                    return true;
                }
            }

            return false;
        }

        private bool AvailabilityСheckBook(uint id) //Проверка id книги
        {
            foreach (var item in books)
            {
                if (item.id == id)
                {
                    return true;
                }
            }

            return false;
        }

        void _UnificationId(Book book) 
        {
            if (AvailabilityСheckBook(book.id))
            {
                book.UnificationId();
                _UnificationId(book);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            UpList(search());
        }

        private List<Book> search()
        {
            List<Book> result = new();

            foreach (var item in books)
            {
                if (item.title.Contains(textBox2.Text))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            open = new OpenFileDialog();
            open.InitialDirectory = "..\\Resources";
            if (open.ShowDialog() == DialogResult.Cancel)
                return;
            textBox3.Text = open.FileName;
        }

        private void Moderating_SizeChanged(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            DeleteBook(GetBook(listBox1.SelectedItem.ToString()), false);
            UpList();

            MessageBox.Show("Книга удалена!");
        }

        private async void DeleteReadersBook(Book book, bool IsChange) 
        {
            if (IsChange) readersChangesBook = new List<Reader>();
            foreach (var item in readers) 
            {
                if (item.books.Contains(book.title)) 
                {
                    item.books.Remove(book.title);
                    if (IsChange) readersChangesBook.Add(item);
                }
            }
            await Task.Run(() => 
            {
                using (antContext db = new())
                {
                    db.Readers.UpdateRange(readers);
                    db.SaveChangesAsync();
                }
            });
        }
    }
}