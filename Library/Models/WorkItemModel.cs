using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using TestIT.Linker.Models.Common;
using TestIT.Linker.Models.Enums;

namespace TestIT.Linker.Models
{
    public class WorkItemModel : ModelBase
    {
        public string Name { get; set; }

        public string EntityTypeName { get; set; }

        public Guid ProjectId { get; set; }

        public Guid SectionId { get; set; }

        public bool? IsAutomated { get; set; }

        public long GlobalId { get; set; }

        public int Duration { get; set; }

        public Guid CreatedById { get; set; }

        public Guid? ModifiedById { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string State { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WorkItemPriority Priority { get; set; }
    }
}
