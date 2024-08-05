using System;
using System.Collections.Generic;


namespace HorizontalNeftMobilaApp
{
    public partial class DepartmentLocation
    {
        public long Id { get; set; }

        public long DepartmentId { get; set; }

        public long LocationId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }


        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();

        public virtual Department Department { get; set; } = null;

        public virtual Location Location{ get; set; } = null;
    }
}


