using System.Collections.Generic;
using System.Windows.Forms;

namespace Library.Class
{
    public class Reader
    {
        protected internal event HandlerLib EReturnBook;
        protected internal event HandlerLib ETakeBook;
        protected internal event HandlerLib ECreateReader;
        private static uint _id = 0;
        public Reader(string name, string password, uint age)
        {
            books = new List<string>();

            id = ++_id;
            Name = name;
            this.password = password;
            Age = age;

            if (Age < 18)
            {
                IsAnAdult = false;
            }
            else
            {
                IsAnAdult = true;
            }

            CallEvent(new AccountEventArgsLib($"Создан новый пользователь. ID - {id}"), ECreateReader);
        }
        public uint id { private set; get; }
        public string Name { private set; get; }
        public bool IsAnAdult { get; private set; }
        public string password { private set; get; }
        public uint Age { private set; get; }
        public bool IsAdmin { private set; get; }
        public List<string> books { private set; get; }
        

        private void CallEvent(AccountEventArgsLib e, HandlerLib handler)
        {
            if (e != null)
                handler?.Invoke(this, e);
        }

        public void ReturnBook(Book book)
        {
            book.Quantity++;
            books.Remove(book.title);
            CallEvent(new AccountEventArgsLib($"Пользователь {this.Name} вернул книгу {book.title}. ID книги - {book.id}"), EReturnBook);
        }

        public void TakeBook(Book book)
        {
            if (book.Quantity <= 0)
            {
                MessageBox.Show("Этой книги нет в наличии");
                return;
            }
            if (books.Contains(book.title))
            {
                MessageBox.Show("Такая книга уже есть");
                return;
            }
            book.Quantity--;
            books.Add(book.title);
            CallEvent(new AccountEventArgsLib($"Пользователь {this.Name} взял книгу {book.title}. ID книги - {book.id}"), ETakeBook);
        }

    }
}
