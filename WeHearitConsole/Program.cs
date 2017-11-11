using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHearitConsole
{
    class Program
    {
        static Page pg;
        static void Main(string[] args)
        {
            pg = new Page();
            while(true)
            {
                OpenMenu();
            }
        }

        public static void OpenMenu()
        {
            Console.WriteLine("post or like ?");
            switch (Console.ReadLine())
            {
                case "post":
                    pg.Post();
                    break;
                case "like":
                    pg.Like();
                    break;

            }
        }
    }
   

   
}
