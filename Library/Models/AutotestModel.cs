using Newtonsoft.Json;
using System;
using TestIT.Linker.Models.Common;

namespace TestIT.Linker.Models
{
    public class AutotestModel : ModelBase
    {
        public long GlobalId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid CreatedById { get; set; }

        public Guid? ModifiedById { get; set; }

        public string ExternalId { get; set; }

        public string LinkToRepository { get; set; }

        public Guid ProjectId { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public string Classname { get; set; }

        public string[] Steps { get; set; } = new string[] { };
    }
}
