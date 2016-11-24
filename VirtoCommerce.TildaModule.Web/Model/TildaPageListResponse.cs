using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtoCommerce.TildaModule.Web.Model
{
    public class TildaPageListResponse
    {
        public string Status { get; set; }

        public TildaPage[] Result { get; set; }
    }
}