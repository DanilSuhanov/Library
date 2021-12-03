using System;
using System.Collections.Generic;

namespace Library.Class
{
    public class Book
    {
        private static uint _id = 0;
        public Book(string title, string desc, string category, int quantity, bool forAdults, string img = "Resources.Def.png")
        {
            this.id = ++_id;
            try
            {
                this.category = category;
                this.title = title;
                this.desc = desc;
                this.img = img;
                Quantity = quantity;
                this.forAdults = forAdults;
            }
            catch
            {
                this.title = "No title" + Convert.ToString(this.id);
            }
        }

        public void UnificationId() 
        {
            this.id++;
        }

        public string title { private set; get; }
        public string desc { private set; get; }
        public uint id { private set; get; }
        public string img { set; get; }
        public string category { private set; get; }
        public int Quantity { set; get; }
        public bool forAdults { set; get; }
    }
}
