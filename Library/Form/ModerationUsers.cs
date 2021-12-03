using Library.Class;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library
{
    public partial class ModerationUsers : Form
    {
        List<Reader> readers;
        List<Book> books;

        Reader reader;

        public ModerationUsers()
        {
            InitializeComponent();

            Up();
            UpBookAsync();

            foreach (var item in readers)
            {
                listBox1.Items.Add(item.Name);
            }
        }

        private async void UpBookAsync()
        {
            await Task.Run(() =>
            {
                using (antContext db = new())
                {
                    books = db.Books.ToList();
                }
            });
        }

        private void Up()
        {
            using (antContext db = new())
            {
                readers = db.Readers.ToList();
            }
        }

        private void UpBooks()
        {
            listBox2.Items.Clear();
            reader = readers.Find((Reader reader) => 
            {
                if (reader.Name == listBox1.SelectedItem.ToString()) return true;
                return false;
            });
            listBox2.Items.AddRange(reader.books.ToArray());
        }

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            UpBooks();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (listBox2.SelectedItem == null)
            {
                MessageBox.Show("Выберите книгу!");
                return;
            }

            Book book = books.Find((Book book) =>
            {
                if (book.title == listBox2.SelectedItem.ToString()) return true;
                return false;
            });

            reader.ReturnBook(book);

            using (antContext db = new antContext())
            {
                db.Readers.Update(reader);
                db.Books.Update(book);
                db.SaveChanges();
            }

            listBox2.Items.Remove(listBox2.SelectedItem.ToString());

            MessageBox.Show("Книга возвращена!");
        }

        private void ModerationUsers_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void ModerationUsers_SizeChanged(object sender, System.EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }
    }
}
