using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Texts2HTML
{
    class Program
    {
        Hashtable phonesToPeople = new Hashtable();
        Hashtable PeopleToTexts = new Hashtable();

        static void Main(string[] args)
        {
            var program = new Program();
            HashSet<Person> allMyFriends = program.parseInput();

            foreach(Person friend in allMyFriends) {
                System.Console.WriteLine(friend.name + " " + friend.phone + " " + friend.msgCount);
            }

            System.Console.WriteLine("Done!");

        }

        public HashSet<Person> parseInput()
        {
            HashSet<Person> knownPeople = new HashSet<Person>();

            Person myself = new Person();
            myself.name = "Me";
            myself.phone = "(202) 469-2765";

            phonesToPeople.Add(myself.phone, myself);

            knownPeople.Add(myself);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("C:\\Users\\Rafael\\Desktop\\smsbackup-2012-09-26.xml"); //* load the XML document from the specified file.

            XmlNodeList smsList = xmlDoc.GetElementsByTagName("s");

            foreach (XmlElement msg in smsList)
            {
                Text thisMsg = new Text();

                String timestamp = msg.GetAttribute("t");
                String phonenumber = msg.GetAttribute("r");
                int direction = int.Parse(msg.GetAttribute("d"));

                Person otherPerson;
                if (phonenumber.Contains("(")) //we know this person's name!!
                {
                    otherPerson = extractPerson(phonenumber);
                    
                }
                else //otherwise this is only a phone number and we dont have a name for this person
                {
                    otherPerson = getPerson(phonenumber);
                }

                otherPerson.msgCount++;

                thisMsg.from = direction == 0 ? myself : otherPerson;
                thisMsg.to = direction == 0 ? otherPerson : myself;
                thisMsg.timestamp = timestamp;
                thisMsg.msg = msg.InnerText;

                if (PeopleToTexts.ContainsKey(otherPerson))
                {
                    ArrayList textBucket = (ArrayList)PeopleToTexts[otherPerson];
                    textBucket.Add(thisMsg);
                    PeopleToTexts[otherPerson] = textBucket;
                }
                else
                {
                    ArrayList textBucket = new ArrayList();
                    textBucket.Add(thisMsg);
                    PeopleToTexts.Add(otherPerson, textBucket);
                }

                knownPeople.Add(otherPerson);

                //System.Console.WriteLine(msg.InnerText);
            }

            return knownPeople;
        }

        private Person extractPerson(String input)
        {
            //format is in the following: "full name(phone number)"

            Person otherPerson;

            String[] split = input.Split('(');

            String name = split[0];
            String phonenumber = split[1].TrimEnd(')');
            phonenumber = phonenumber.Replace("-", "");

            otherPerson = getPerson(phonenumber);

            if (otherPerson.name == "Unknown")
            {
                otherPerson.name = name;
            }


            return otherPerson;
        }

        private Person getPerson(String phonenumber)
        {
            Person otherPerson;

            phonenumber = parsePhoneNumber(phonenumber);

            if (phonesToPeople.ContainsKey(phonenumber)) //we have gotten a text from this person before
            {
                otherPerson = (Person)phonesToPeople[phonenumber];
            }
            else //first text from this person
            {
                otherPerson = new Person();
                otherPerson.phone = phonenumber;
                otherPerson.msgCount = 0;
                otherPerson.name = "Unknown";

                phonesToPeople.Add(phonenumber, otherPerson);
            }

            return otherPerson;
        }

        private String parsePhoneNumber(String phonenumber)
        {
            if(phonenumber == "") return "";

            int startIndex = phonenumber[0].Equals('+') ? 2 : 0;

            if (phonenumber.Substring(startIndex).Length != 10) return phonenumber; //special phone numbers

            String areacode = phonenumber.Substring(startIndex, 3); //first 3 numbers
            String middle = phonenumber.Substring(startIndex + 3, 3); //next 3 numbers
            String last = phonenumber.Substring(startIndex + 6, 4); //last four numbers

            return "(" + areacode + ") " + middle + "-" + last;
            
        }
    }
}
