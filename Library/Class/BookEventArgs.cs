namespace Library.Class
{
    public delegate void HandlerBook(object sender, BookEventArgs e);
    public class BookEventArgs
    {
        public Book book { private set; get; }
        public BookEventArgs(Book book)
        {
            this.book = book;
        }
    }
}
