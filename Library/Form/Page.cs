using Library.Class;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Library
{
    public partial class Page : Form
    {
        PictureBox[] pictureBoxes;
        TextBox[] textBoxes;
        RichTextBox[] richTextBoxes;
        Button[] buttons;

        Reader reader;

        List<Book> books;

        public static event HandlerBook EAddCart;
        public Page(Reader reader)
        {
            InitializeComponent();
            this.reader = reader;
            books = Session.main.SortBooks;

            pictureBoxes = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6 };
            textBoxes = new TextBox[] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 };
            richTextBoxes = new RichTextBox[] { richTextBox1, richTextBox2, richTextBox3, richTextBox4, richTextBox5, richTextBox6 };
            buttons = new Button[] { button1, button2, button3, button4, button5, button6 };
        }

        public void UpCart() 
        {
            button10.Text = $"Корзина: {Session.shopListCount}";
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            Name = $"Страница {Session.PageNow}";
            if (Session.PageNow != Session.PageCount) button8.Visible = true;
            label1.Text = reader.Name;
            UpCart();

            Cart.EClearCart += Cart_EClearCart;
            Cart.ERemoveFCart += Cart_ERemoveFCart;
            
            if (books.Count == PageBook(1))
            {
                BookBuild(0);
            }
            else if (books.Count == PageBook(2))
            {
                for (int i = 0; i <= 1; i++)
                {
                    BookBuild(i);
                }
            }
            else if (books.Count == PageBook(3)) 
            {
                for (int i = 0; i <= 2; i++)
                {
                    BookBuild(i);
                }
            }
            else if (books.Count == PageBook(4))
            {
                for (int i = 0; i <= 3; i++)
                {
                    BookBuild(i);
                }
            }
            else if(books.Count == PageBook(5))
            {
                for (int i = 0; i <= 4; i++)
                {
                    BookBuild(i);
                }
            }
            else if(books.Count == PageBook(6))
            {
                for (int i = 0; i <= 5; i++)
                {
                    BookBuild(i);
                }
            }
        }

        private void Cart_ERemoveFCart(object sender, BookEventArgs e)
        {
            Session.shopListCount = 0;
            buttons[NumberBook(e.book)].Enabled = true;
            buttons[NumberBook(e.book)].Text = "В корзину";
        }

        private int NumberBook(Book book)
        {
            try
            {
                if (books[PageBook(0)] == book) return 0;
                if (books[PageBook(1)] == book) return 1;
                if (books[PageBook(2)] == book) return 2;
                if (books[PageBook(3)] == book) return 3;
                if (books[PageBook(4)] == book) return 4;
                if (books[PageBook(5)] == book) return 5;
            }
            catch { }

            return -1;
        }

        private void Cart_EClearCart(object sender, AccountEventArgsLib e)
        {
            button10.Text = "Корзина: 0";
        }

        private void UnEnabled(int x, string reason)
        {
            buttons[x].Text = reason;
            buttons[x].Enabled = false;
        }

        private void Page_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private int PageBook(int num)
        {
            return (num + 4) * Session.PageNow;
        }

        private void CheckBookList(int num, int numOnForm)
        {
            if (reader.books.Contains(books[num].title)) UnEnabled(numOnForm, "Книга уже есть у вас");
            if (Session.main.cart.shopList.Contains(books[num])) UnEnabled(numOnForm, "В корзине");
            if (books[num].forAdults == true && reader.IsAnAdult == false) UnEnabled(numOnForm, "+18");
        }

        private void BookBuild(int num) 
        {
            buttons[num].Visible = true;

            CheckBookList(PageBook(num), num + 1);

            pictureBoxes[num].Visible = true;
            pictureBoxes[num].Image = Image.FromFile(books[PageBook(num)].img);

            richTextBoxes[num].Visible = true;
            richTextBoxes[num].Text = books[PageBook(num)].desc + "\nКоличество: " + books[PageBook(num)].Quantity;

            textBoxes[num].Visible = true;
            textBoxes[num].Text = books[PageBook(num)].title;
        }

        private void ClickBook(int num)
        {
            EAddCart(this, new(books[PageBook(num)]));
            button10.Text = $"Корзина: {++Session.shopListCount}";
            UnEnabled(num, "В корзине");
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            ClickBook(0);
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            ClickBook(1);
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            ClickBook(2);
        }

        private void button4_Click(object sender, System.EventArgs e)
        {
            ClickBook(3);
        }

        private void button5_Click(object sender, System.EventArgs e)
        {
            ClickBook(4);
        }

        private void button6_Click(object sender, System.EventArgs e)
        {
            ClickBook(5);
        }

        private void button10_Click(object sender, System.EventArgs e) //Корзина
        {
            Session.main.cart.Show();
        }

        private void button9_Click(object sender, System.EventArgs e) //Мои книги
        {
            Session.main.myBooks.Show();
        }

        private void button7_Click(object sender, System.EventArgs e) //Назад
        {
            Hide();
            Session.main.Show();
        }

        private void button8_Click(object sender, System.EventArgs e) //Вперёд
        {
            Hide();
            Session.PageNow++;
            Session.pages[Session.PageNow] = new Page(reader);
            Session.pages[Session.PageNow].Show();
        }
    }
}