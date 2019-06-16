using System.Xml;
using UnityEngine.Assertions;

namespace ArsenStudio.Linter
{
    public class RuleSet
    {
        public void Load()
        {
            XmlDocument doc = new XmlDocument();  
            doc.Load("bookstore.xml");  
            XmlNode root = doc.DocumentElement;
            

            Assert.AreEqual(root.Name, "ruleset");

            root.SelectNodes("child::include-assembly");
            root.SelectNodes("child::exclude-assembly");
        }
    }
}

