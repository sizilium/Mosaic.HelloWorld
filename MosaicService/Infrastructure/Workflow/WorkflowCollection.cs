using System.Collections.Generic;

namespace MosaicSample.Infrastructure.Workflow
{
    public class WorkflowCollection
    {
        public IEnumerable<Workflow> Workflows { get; set; }
    }

    public class Workflow
    {
        public string Barcode { get; set; }
        public IEnumerable<string> WorkFlow { get; set; }
    }
}
