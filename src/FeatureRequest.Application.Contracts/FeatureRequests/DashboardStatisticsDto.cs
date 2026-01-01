using System.Collections.Generic;

namespace FeatureRequest.FeatureRequests
{
    public class DashboardStatisticsDto
    {
        public int TotalRequests { get; set; }
        public int TotalVotes { get; set; }
        public int TotalComments { get; set; }
        public int TotalUsers { get; set; }
        
        public List<CategoryStatDto> CategoryStats { get; set; } = new();
        public List<StatusStatDto> StatusStats { get; set; } = new();
        public List<FeatureRequestDto> TopVotedRequests { get; set; } = new();
    }

    public class CategoryStatDto
    {
        public string Category { get; set; }
        public int RequestCount { get; set; }
        public int TotalVotes { get; set; }
    }

    public class StatusStatDto
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }
}
