using System;
using System.ComponentModel.DataAnnotations;

namespace FeatureRequest.FeatureRequests
{
    public class CreateCommentDto
    {
        [Required]
        public Guid FeatureRequestId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Yorum 2 ile 500 karakter arasında olmalıdır.")]
        public string CommentText { get; set; }
    }
}