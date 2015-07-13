using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SpellViewer
{
    public class spell
    {
        public string name { get; set; }
        
        public string level { get; set; }
        
        public string school { get; set; }
        
        public string ritual { get; set; }
        
        public string time { get; set; }
        
        public string range { get; set; }
        
        public string components { get; set; }
        
        public string duration { get; set; }

        public string classes { get; set; }

        public IEnumerable<string> text { get; set; }

        public IEnumerable<string> roll { get; set; }
    }

    public class SpellsManager
    {
        public spell[] spells { get; set; }

        public static SpellsManager Load(Stream stream)
        {
            SpellsManager context = new SpellsManager();
            /*XmlSerializer serializer = new XmlSerializer(typeof(SpellsManager));
            
            using (StreamReader reader = new StreamReader(path))
            {
                context = (SpellsManager)serializer.Deserialize(reader);
            }*/

            using (StreamReader reader = new StreamReader(stream))
            {
                XDocument doc = XDocument.Load(reader);
                context.spells = doc.Element("spells").Elements("spell").Select(f => new spell()
                {
                    name = f.Element("name").Value,
                    level = f.Element("level").Value,
                    school = f.Element("school").Value,
                    ritual = f.Element("ritual").Value,
                    time = f.Element("time").Value,
                    range = f.Element("range").Value,
                    components = f.Element("components").Value,
                    duration = f.Element("duration").Value,
                    classes = f.Element("classes").Value,
                    text = f.Elements("text").Select(t=> { return (t.FirstNode is XNode ? t.FirstNode.ToString() : ""); } ),
                    roll = f.Elements("roll").Select(t => { return (t.FirstNode is XNode ? t.FirstNode.ToString() : ""); } ),
                }).ToArray();
            }
            return context;
        }
    }
}
