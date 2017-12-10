using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPCoreWebApplication.Models.Catalog
{
    public class AssetIndexListingModel
    {
        public int Id {get; set;}
        public string ImageURL { get; set; }
        public string Title { get; set; }
        public string AuthorOrDirector { get; set; }
        public string Type { get; set; }
        public string DeweyCallNumber { get; set; }
        public string NumberOfCopies { get; set; }

    }
}
