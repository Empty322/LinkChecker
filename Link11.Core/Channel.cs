using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Link11.Core
{
    [Serializable] 
    public class ch
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("freq")]
        public float freq { get; set; }
        [XmlAttribute("trakt")]
        public string Trakt { get; set; }
        [XmlAttribute("seanse_dir")]
        public string Directory { get; set; }
        [XmlAttribute("desc")]
        public string Desc { get; set; }
        [XmlAttribute("server")]
        public string Server { get; set; }
        [XmlAttribute("record")]
        public string Record { get; set; }
        [XmlAttribute("carrier")]
        public float Carrier { get; set; }
        [XmlAttribute("filter")]
        public int Filter { get; set; }
        [XmlAttribute("invert")]
        public bool Invert { get; set; }
        public ch() { }
    }
}
