using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManager.Models
{
    class Book
    {
        public string Isbn { set; get; }
        public string Name { set; get; }
        public string Publisher { set; get; }
        public int Page { set; get; }
        public int UserId { set; get; }
        public string UserName { set; get; }
        public bool isBorrowed { set; get; }
        public DateTime BorrowedAt { set; get; }
    }
}
