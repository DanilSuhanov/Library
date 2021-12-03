using Library.Class;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Library
{
    public partial class Main : Form
    {
        PictureBox[] pictureBoxes;
        TextBox[] textBoxes;
        RichTextBox[] richTextBoxes;
        Button[] buttons;

        List<Book> books;
        public List<Book> SortBooks;

        Reader reader;

        public Cart cart;
        public MyBooks myBooks;

        public event HandlerBook EAddCart;
        public Main(Reader reader)
        {
            InitializeComponent();
            this.reader = reader;
            Up();

            pictureBoxes = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4 };
            textBoxes = new TextBox[] { textBox1, textBox2, textBox3, textBox4 };
            richTextBoxes = new RichTextBox[] { richTextBox1, richTextBox2, richTextBox3, richTextBox4 };
            buttons = new Button[] { button3, button4, button5, button6 };
        }

        private void Главная_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        public void UpCart()
        {
            button1.Text = $"Корзина: {Session.shopListCount}";
        }

        private void Главная_Load(object sender, EventArgs e)
        {
            Cart.EClearCart += Cart_EClearCart;
            Cart.ERemoveFCart += Cart_ERemoveFCart;
            cart = new(reader);
            myBooks = new(reader);
            SortBooks = books;
            Initializing();
            Session.PageNow++;
        }

        private void Initializing()
        {
            if (SortBooks.Count > 4)
            {
                button2.Visible = true;
                double Count = SortBooks.Count;
                double PageCountD = (Count - 4) / 6;
                Session.PageCount = (int)Math.Round(PageCountD);
                PageCountD -= Session.PageCount;
                if (PageCountD > 0) Session.PageCount++;
                Session.pages = new Page[Session.PageCount];
                Session.pages[0] = new Page(reader);
            }
            label1.Text = reader.Name;
            switch (SortBooks.Count)
            {
                case 1:
                    BookBuild(0);
                    break;
                case 2:
                    for (int i = 0; i <= 1; i++) BookBuild(i);
                    break;
                case 3:
                    for (int i = 0; i <= 2; i++) BookBuild(i);
                    break;
                case 4:
                    for (int i = 0; i <= 3; i++) BookBuild(i);
                    break;
                default:
                    break;
            }
            if (SortBooks.Count > 4)
            {
                for (int i = 0; i <= 3; i++) BookBuild(i);
            }
        }

        private void Cart_ERemoveFCart(object sender, BookEventArgs e)
        {
            Session.shopListCount = 0;
            try
            {
                EnabledBook(NumberBook(e.book));
            }
            catch { }
            UpCart();
        }

        private void EnabledBook(int num)
        {
            buttons[num].Enabled = true;
            buttons[num].Text = "В корзину";
        }

        private int NumberBook(Book book)
        {
            if (SortBooks[0] == book) return 0;
            if (SortBooks[1] == book) return 1;
            if (SortBooks[2] == book) return 2;
            if (SortBooks[3] == book) return 3;

            return -1;
        }

        private void Cart_EClearCart(object sender, AccountEventArgsLib e)
        {
            button1.Text = "Корзина: 0";
        }

        private void CheckBookList(int num) 
        {
            if (SortBooks[num].forAdults == true && reader.IsAnAdult == false) UnEnabled(num, "+18");
            if (SortBooks[num].Quantity <= 0) UnEnabled(num, "Книги нет в наличии");
            if (cart.shopList.Contains(SortBooks[num])) UnEnabled(num, "В корзине");
            if (reader.books.Contains(SortBooks[num].title)) UnEnabled(num, "Книга уже есть у вас");
        }

        private void BookBuild(int num)
        {
            buttons[num].Visible = true;

            CheckBookList(num);

            pictureBoxes[num].Visible = true;
            pictureBoxes[num].Image = Image.FromFile(SortBooks[num].img);

            richTextBoxes[num].Visible = true;
            richTextBoxes[num].Text = SortBooks[num].desc + "\nКоличество: " + SortBooks[num].Quantity;

            textBoxes[num].Visible = true;
            textBoxes[num].Text = SortBooks[num].title;
        }

        private void Up()
        {
            using (antContext db = new antContext())
            {
                books = db.Books.ToList();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Session.pages[0].UpCart();
            Session.pages[0].Show();
        }

        private void UnEnabled(int x, string reason) 
        {
            buttons[x].Text = reason;
            buttons[x].Enabled = false;
        }

        private void ClickButton(int num)
        {
            EAddCart(this, new(SortBooks[num]));
            button1.Text = $"Корзина: {++Session.shopListCount}";
            UnEnabled(num, "В корзине");
        }

        private void button3_Click(object sender, EventArgs e) 
        {
            ClickButton(0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClickButton(1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClickButton(2);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ClickButton(3);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cart.Show();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == 0)
            {
                SortBooks = books;
            }
            else
            {
                SortBooks = books.Where((Book book) =>
                {
                    if (book.category == listBox1.SelectedItem.ToString())
                    {
                        return true;
                    }
                    return false;
                }).ToList();
            }

            ClearForm();
            Initializing();
        }

        private void ClearForm() 
        {
            AllUnVisible();
            AllEnable();
        }

        private void AllEnable() 
        {
            for (int i = 0; i <= 3; i++) EnabledBook(i);
        }

        private void AllUnVisible()
        {
            foreach (var item in buttons) item.Visible = false;
            foreach (var item in pictureBoxes) item.Visible = false;
            foreach (var item in richTextBoxes) item.Visible = false;
            foreach (var item in textBoxes) item.Visible = false;

            button2.Visible = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            myBooks.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Session.main = null;
            Session.PageCount = 0;
            Session.PageNow = 0;
            Session.pages = null;
            Session.shopListCount = 0;
            new Form1(false).Show();
            this.Hide();
            ClearAuthTemp();
        }

        private void ClearAuthTemp() 
        {
            File.Delete("AuthTemp.txt");
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }
    }
}