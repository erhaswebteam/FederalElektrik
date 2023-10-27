using Grand.Core.Domain.Topics;
using Grand.Framework.Mvc.Models;
using System.Collections.Generic;

namespace Grand.Plugin.Rewards.RewardOperations.Models
{
    public partial class TopicModel : BaseGrandEntityModel
    {
        public string SystemName { get; set; }

        public bool IncludeInSitemap { get; set; }

        public bool IsPasswordProtected { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string MetaTitle { get; set; }

        public string SeName { get; set; }

        public string TopicTemplateId { get; set; }

        public List<Topic> ParentTopics { get; set; }
    }
}