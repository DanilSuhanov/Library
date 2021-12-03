namespace Library.Class
{
    public delegate void HandlerLib(object sender, AccountEventArgsLib e);
    public class AccountEventArgsLib
    {
        public string Message { get; private set; }
        public AccountEventArgsLib(string Message)
        {
            this.Message = Message;
        }
    }
}
