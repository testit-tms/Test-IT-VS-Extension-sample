using System;

namespace TestIT.Linker.Models.Common
{
    public abstract class ModelBase
    {
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }
    }
}
