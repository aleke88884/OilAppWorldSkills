using System;
using System.Collections.Generic;
using System.Text;

namespace HorizontalNeftMobilaApp
{
    public class Asset
    {
        public long Id { get; set; }
        public string AssetSn { get; set; }
        public string AssetName { get; set; }
        public long DepartmentLocationId { get; set; }
        public long EmployeeId { get; set; }
        public long AssetGroupId { get; set; }
        public string Description { get; set; }
        public DateTime? WarrantyDate { get; set; }
        public virtual AssetGroup AssetGroup { get; set; }
        public virtual DepartmentLocation DepartmentLocation { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
