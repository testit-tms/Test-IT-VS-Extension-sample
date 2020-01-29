using System;
using System.Collections.Generic;
using TestIT.Linker.Models.Common;

namespace TestIT.Linker.Models.Project
{
    public class ProjectModel : ModelBase
    {
        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid CreatedById { get; set; }

        public Guid? ModifiedById { get; set; }

        public long GlobalId { get; set; }

        public string Description { get; set; }

        public string EntityTypeName { get; set; }

        public string Name { get; set; }
    }
}
