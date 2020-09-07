using BookManager.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BookManager.ViewModels
{
    class DataManager
    {
        public static List<Book> Books = new List<Book>();
        public static List<User> Users = new List<User>();

        static DataManager()
        {
            // Load();
            LoadJson();
        }

        private static void LoadJson()
        {
            try
            {
                string strBookValueJson = File.ReadAllText(@"./Books.json");
                JObject jsonBookObj = JObject.Parse(strBookValueJson);
                Books = (from item in jsonBookObj["books"]["book"]
                         select 
                         new Book()
                         {
                             Isbn = item["isbn"].ToString(),
                             Name = item["name"].ToString(),
                             Publisher = item["publisher"].ToString(),
                             Page = int.Parse(item["page"].ToString()),
                             BorrowedAt = DateTime.Parse(item["borrowedat"].ToString()),
                             isBorrowed = item["isborrowed"].ToString() != "0" ? true : false,
                             UserId = int.Parse(item["userid"].ToString()),
                             UserName = item["username"].ToString().Replace("{", "").Replace("}","")
                         }
                         ).ToList<Book>();

                string strUserValueJson = File.ReadAllText(@"./Users.json");
                JObject jsonUserObj = JObject.Parse(strUserValueJson);
                Users = (from item in jsonUserObj["users"]["user"]
                         select new User()
                         {
                             Id = int.Parse(item["id"].ToString()),
                             Name = item["name"].ToString()
                         }
                         ).ToList<User>();
            }
            catch(Exception ex)
            {
                MessageBox.Show("파일이 누락되었습니다!!!");
                Save();
                Load(); //다시 불러들이기
            }
        }

        public static void SaveJson()
        {
            var jBookArray = new JArray();
            foreach (var item in Books)
            {
                var jBookObj = new JObject();
                jBookObj.Add("isbn", item.Isbn);
                jBookObj.Add("name", item.Name);
                jBookObj.Add("publisher", item.Publisher);
                jBookObj.Add("page", item.Page);
                jBookObj.Add("borrowedat", item.BorrowedAt.ToLongDateString());
                jBookObj.Add("isborrowed", item.isBorrowed ? "1" : "0");
                jBookObj.Add("userid", item.UserId);
                jBookObj.Add("username", item.UserName);
                jBookArray.Add(jBookObj);
            }

            var jBookArrayObj = new JObject();
            jBookArrayObj.Add("book", jBookArray);
            var jBooksObj = new JObject();
            jBooksObj.Add("books", jBookArrayObj);

            var jUserArray = new JArray();
            foreach(var item in Users)
            {
                var jUserObj = new JObject();
                jUserObj.Add("id", item.Id);
                jUserObj.Add("name", item.Name);
                jUserArray.Add(jUserObj);
            }

            var jUserArrayObj = new JObject();
            jUserArrayObj.Add("user", jUserArray);
            var jUsersObj = new JObject();
            jUsersObj.Add("users", jUserArrayObj);

            // 저장
            File.WriteAllText(@"./Books.json", jBooksObj.ToString());
            File.WriteAllText(@"./Users.json", jUsersObj.ToString());
        }

        public static void Load()
        {
            LoadJson();
            return;

            try
            {
                string booksOutput = File.ReadAllText(@"./Books.xml");
                XElement booksXElement = XElement.Parse(booksOutput);
                Books = (from item in booksXElement.Descendants("book")
                         select new Book()
                         {
                             Isbn = item.Element("isbn").Value,
                             Name = item.Element("name").Value,
                             Publisher = item.Element("publisher").Value,
                             Page = int.Parse(item.Element("page").Value),
                             BorrowedAt = DateTime.Parse(item.Element("borrowedAt").Value),
                             isBorrowed = item.Element("isBorrowed").Value != "0" ? true : false,
                             UserId = int.Parse(item.Element("userId").Value),
                             UserName = item.Element("userName").Value
                         }).ToList<Book>();

                string usersOutput = File.ReadAllText(@"./Users.xml");
                XElement usersXElement = XElement.Parse(usersOutput);
                Users = (from item in usersXElement.Descendants("user")
                         select new User()
                         {
                             Id = int.Parse(item.Element("id").Value),
                             Name = item.Element("name").Value
                         }).ToList<User>();
            }
            catch (FileLoadException exception)
            {
                // 파일이 없으면 예외 발생: 새로운 파일 생성
                Save();
            }
        }

        public static void Save()
        {
            SaveJson();
            return;

            // 도서 XML 생성
            string booksOutput = "";
            booksOutput += "<books>\n";
            foreach (var item in Books)
            {
                booksOutput += "<book>\n";
                booksOutput += "  <isbn>" + item.Isbn + "</isbn>\n";
                booksOutput += "  <name>" + item.Name + "</name>\n";
                booksOutput += "  <publisher>" + item.Publisher + "</publisher>\n";
                booksOutput += "  <page>" + item.Page + "</page>\n";
                booksOutput += "  <borrowedAt>" + item.BorrowedAt.ToLongDateString() + "</borrowedAt>\n";
                booksOutput += "  <isBorrowed>" + (item.isBorrowed ? 1 : 0) + "</isBorrowed>\n";
                booksOutput += "  <userId>" + item.UserId + "</userId>\n";
                booksOutput += "  <userName>" + item.UserName + "</userName>\n";
                booksOutput += "</book>\n";
            }
            booksOutput += "</books>";

            // 사용자 JSON 생성
            string usersOutput = "";
            usersOutput += "<users>\n";
            foreach (var item in Users)
            {
                usersOutput += "<user>\n";
                usersOutput += "  <id>" + item.Id + "</id>\n";
                usersOutput += "  <name>" + item.Name + "</name>\n";
                usersOutput += "</user>\n";
            }
            usersOutput += "</users>";

            // 저장
            File.WriteAllText(@"./Books.xml", booksOutput);
            File.WriteAllText(@"./Users.xml", usersOutput);
        }
    }
}
