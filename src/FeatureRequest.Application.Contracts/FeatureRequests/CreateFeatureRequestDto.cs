using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FeatureRequest.FeatureRequests
{
    public class CreateFeatureRequestDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        public FeatureRequestCategory CategoryId { get; set; }

    }
}
