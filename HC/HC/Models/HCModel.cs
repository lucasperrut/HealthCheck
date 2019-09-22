using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;

namespace WebApplication1.HC.Models
{
    internal class HCModel
    {
        public string Status { get; set; }
        public IEnumerable<HCResultModel> Results { get; set; }

        internal class HCResultModel
        {
            public string Name { get; set; }
            public string Status { get; set; }
            public string Description { get; set; }
        }
    }
}
