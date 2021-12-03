using Library.Class;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Library
{
    public partial class Cart : Form
    {
        Reader reader;

        public List<Book> shopList;

        public static event HandlerLib EClearCart;
        public static event HandlerBook ERemoveFCart;
        public Cart(Reader reader)
        {
            InitializeComponent();

            shopList = new List<Book>();

            Session.main.EAddCart += Main_EAddCart1;
            Page.EAddCart += Main_EAddCart1;

            this.reader = reader;
        }

        private void Main_EAddCart1(object sender, BookEventArgs e)
        {
            AddList(e.book);
        }

        private void CallEvent(AccountEventArgsLib e, HandlerLib handler)
        {
            if (e != null)
                handler?.Invoke(this, e);
        }

        private void AddList(Book book) 
        {
            listBox1.Items.Add(book.title);
            shopList.Add(book);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items == null)
            {
                MessageBox.Show("Положите книги в корзину");
                return;
            }
            if (shopList == null) return;
            foreach (var item in shopList)
            {
                reader.TakeBook(item);
            }

            using (antContext db = new antContext())
            {
                db.Readers.Update(reader);
                db.Books.UpdateRange(shopList);
                db.SaveChanges();
            }

            MessageBox.Show("Заказ оформлен!");

            ClearAll();

            CallEvent(new("Корзина пуста!"), EClearCart);
        }

        private void ClearAll() 
        {
            listBox1.Items.Clear();
            shopList.Clear();
            Session.shopListCount = 0;
        }

        private Book FindBook(string title)
        {
            foreach (var item in shopList)
                if (item.title == title) return item;

            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите объект!");
                return;
            }
            Book book = FindBook(listBox1.SelectedItem.ToString());
            shopList.Remove(book);
            listBox1.Items.Remove(listBox1.SelectedItem);
            ERemoveFCart(this, new(book));

            MessageBox.Show("Книга удалена из корзины!");
        }

        private void Cart_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason.ToString() == "UserClosing")
            {
                e.Cancel = true;
            }
            Hide();
        }

        private void Cart_SizeChanged(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }
    }
}
