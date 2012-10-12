using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Texts2HTML
{
    class HTMLGenerator
    {
        public bool start(HashSet<Person> friends, Hashtable PeopleToTexts, String output) {
            StringWriter stringWriter = new StringWriter();
            
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
	        {
                foreach (Person friend in friends)
                {
                    LinkedList<Text> textBucket = (LinkedList<Text>)PeopleToTexts[friend];

                    writer.RenderBeginTag(HtmlTextWriterTag.H2);
                    writer.Write(friend.phone + " - " + friend.name + " - " + friend.msgCount + " Messages total");
                    writer.RenderEndTag();

                    foreach (Text msg in textBucket)
                    {
                        writer.Write(msg.from.name + ": " + msg.msg + "   " + msg.timestamp);
                        writer.RenderBeginTag(HtmlTextWriterTag.Br);
                        writer.RenderEndTag();
                    }
                }
	        }

            try
            {
                System.IO.File.WriteAllText(output, stringWriter.ToString());
            }
            catch
            {
                System.Console.WriteLine("Could not write to output file!");
                System.Environment.Exit(5);
            }

            return true;
        }
    }
}
