using System;
using System.Collections.Generic;
using System.Text;

namespace HorizontalNeftMobilaApp
{
    public class AssetGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Asset> Assets { get; set; }
    }
}
