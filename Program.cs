using System;
using System.Collections;
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

            verifyInput(args);

            String path = args[0];

            Hashtable PeopleToTexts = new Hashtable();
            Parser parser = new Parser();
            HashSet<Person> allMyFriends = parser.parseInput(path, PeopleToTexts);

            String outpath = args[1];

            HTMLGenerator gen = new HTMLGenerator();
            gen.start(allMyFriends, PeopleToTexts, outpath);

            /*
            foreach(Person friend in allMyFriends) {
                System.Console.WriteLine(friend.name + " " + friend.phone + " " + friend.msgCount);
            }
            */

            System.Console.WriteLine("Done!");
        }

        public static void verifyInput(string[] args)
        {
            if (args.Length != 2)
            {
                System.Console.WriteLine("Please supply 2 and only 2 arguments: path to input xml and path to output file");
                System.Environment.Exit(1);
            }

            string[] split = args[0].Split('.');
            string ext = split[split.Length-1].ToLower();

            if(!ext.Equals("xml")) {
                System.Console.WriteLine("first argument must be an xml file");
                System.Environment.Exit(2);
            }

            split = args[1].Split('.');
            ext = split[split.Length - 1].ToLower();

            if (!ext.Equals("html"))
            {
                System.Console.WriteLine("It is recommended that the second argument be an HTML file!");
                System.Console.WriteLine("...continuing anyways...");
            }
        }
    }
}
