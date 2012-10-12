using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Texts2HTML
{
    class Program
    {
        static void Main(string[] args)
        {
            String path = "C:\\Users\\Rafael\\Desktop\\smsbackup-2012-09-26.xml";

            Parser parser = new Parser();
            HashSet<Person> allMyFriends = parser.parseInput(path);

            foreach(Person friend in allMyFriends) {
                System.Console.WriteLine(friend.name + " " + friend.phone + " " + friend.msgCount);
            }

            System.Console.WriteLine("Done!");

        }
    }
}
