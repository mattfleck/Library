using System.Collections.Generic;

namespace ASPCoreWebApplication.Models.Catalog
{
    public class AssetIndexModel
    {
        public IEnumerable<AssetIndexListingModel> Assets { get; set; }
    }
}
