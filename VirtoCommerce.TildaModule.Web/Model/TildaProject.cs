namespace VirtoCommerce.TildaModule.Web.Model
{
    public class TildaProject
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Descr { get; set; }

        public string[] Css { get; set; }

        public string[] Js { get; set; }
    }
}