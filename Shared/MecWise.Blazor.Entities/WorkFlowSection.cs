using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    public class WorkFlowSection
    {
        public string WFRemark { get; set; }
        public List<ClarifySelectRecip> ClarifySelectRecipList { get; set; }
        public List<OwnRouting> OwnRoutingList { get; set; }
        public List<WorkFlowSectionRecipStatus> RecipStatusList { get; set; }
    }

    public class OwnRouting
    {
        public string RecipId { get; set; }
        public string RecipName { get; set; }
        public string RecipAction { get; set; }
    }

    public class ClarifySelectRecip
    {
        public string RecipId { get; set; }
        public string RecipName { get; set; }
        public string RecipAction { get; set; }
    }

    public class WorkFlowSectionRecipStatus
    {
        public string RecipId { get; set; }
        public string RecipName { get; set; }
        public string RecipAction { get; set; }
    }
}
