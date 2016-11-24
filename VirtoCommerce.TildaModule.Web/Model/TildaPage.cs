using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtoCommerce.TildaModule.Web.Model
{
    public class TildaPage
    {
        public int Id { get; set; }

        public int ProjectId { get; set; } 

        public string Title { get; set; }

        public string Descr { get; set; }

        public string Img { get; set; }

        public string FeatureImg { get; set; }

        public string Alias { get; set; }

        public DateTime Date { get; set; }

        public int Sort { get; set; }

        public int Published { get; set; }

        public string Html { get; set; }

        public string FileName { get; set; }
    }
}