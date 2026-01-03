using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FeatureRequest.FeatureRequests
{
    public class CreateFeatureRequestDto
    {
        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Başlık 5 ile 200 karakter arasında olmalıdır.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Açıklama 20 ile 2000 karakter arasında olmalıdır.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public FeatureRequestCategory CategoryId { get; set; }
    }
}
