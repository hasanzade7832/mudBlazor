// Models/PettyCash/PettyCashFormModel.cs
using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.PettyCash
{
    public class PettyCashFormModel
    {
        [Required(ErrorMessage = "عنوان تنخواه الزامی است.")]
        public string Title { get; set; } = string.Empty;
    }
}
