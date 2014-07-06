using System.Collections.Generic;

namespace Jeegoordah.Tests.Integration.Common
{
    class TestEvent
    {
        public TestEvent()
        {
            Name = "";
            Description = "";
            StartDate = "";
            Bros = new List<string>();
        }

        public string Name { get; set; }
        public string StartDate { get; set; }
        public string Description { get; set; }
        public List<string> Bros { get; set; }
    }
}