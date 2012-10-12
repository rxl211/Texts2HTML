using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Texts2HTML
{
    class Parser
    {
        private Hashtable phonesToPeople = new Hashtable();
        private Hashtable PeopleToTexts;

        public HashSet<Person> parseInput(String xmlPath, Hashtable PeopleToTexts)
        {
            HashSet<Person> knownPeople = new HashSet<Person>();
            this.PeopleToTexts = PeopleToTexts;

            Person myself = initilizeMyself();
            knownPeople.Add(myself);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath); //* load the XML document from the specified file.

            XmlNodeList smsList = xmlDoc.GetElementsByTagName("s");

            foreach (XmlElement msg in smsList)
            {
                String phonenumber = msg.GetAttribute("r");

                Person otherPerson = getOtherPerson(phonenumber);
                otherPerson.msgCount++;

                Text thisMsg = setTextValues(msg, myself, otherPerson);
                addTextToBucket(otherPerson, thisMsg);

                knownPeople.Add(otherPerson);
            }

            return knownPeople;
        }

        private void addTextToBucket(Person otherPerson, Text thisMsg)
        {
            if (PeopleToTexts.ContainsKey(otherPerson))
            {
                LinkedList<Text> textBucket = (LinkedList<Text>)PeopleToTexts[otherPerson];
                textBucket.AddFirst(thisMsg);
                PeopleToTexts[otherPerson] = textBucket;
            }
            else
            {
                LinkedList<Text> textBucket = new LinkedList<Text>();
                textBucket.AddFirst(thisMsg);
                PeopleToTexts.Add(otherPerson, textBucket);
            }
        }

        private Person getOtherPerson(String phonenumber)
        {
            Person otherPerson;
            if (phonenumber.Contains("(")) //we know this person's name!!
            {
                otherPerson = extractPerson(phonenumber);

            }
            else //otherwise this is only a phone number and we dont have a name for this person
            {
                otherPerson = getPerson(phonenumber);
            }

            return otherPerson;
        }

        private Text setTextValues(XmlElement msg, Person myself, Person otherPerson)
        {
            Text thisMsg = new Text();

            String timestamp = msg.GetAttribute("t");
            int direction = int.Parse(msg.GetAttribute("d"));

            thisMsg.from = direction == 0 ? myself : otherPerson;
            thisMsg.to = direction == 0 ? otherPerson : myself;
            thisMsg.timestamp = timestamp;
            thisMsg.msg = msg.InnerText;

            return thisMsg;
        }

        private Person initilizeMyself()
        {
            Person myself = new Person();
            myself.name = "Me";
            myself.phone = "";

            phonesToPeople.Add(myself.phone, myself);

            return myself;
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
            if (phonenumber == "") return "";

            int startIndex = phonenumber[0].Equals('+') ? 2 : 0;

            if (phonenumber.Substring(startIndex).Length != 10) return phonenumber; //special phone numbers

            String areacode = phonenumber.Substring(startIndex, 3); //first 3 numbers
            String middle = phonenumber.Substring(startIndex + 3, 3); //next 3 numbers
            String last = phonenumber.Substring(startIndex + 6, 4); //last four numbers

            return "(" + areacode + ") " + middle + "-" + last;

        }
    }
}
