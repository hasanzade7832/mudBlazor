using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.TaskManagement;

public class CreateTaskDto
{
    [Required(ErrorMessage = "عنوان تسک الزامی است.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "توضیحات تسک الزامی است.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "تاریخ مهلت الزامی است.")]
    public DateTime Deadline { get; set; }

    [Required(ErrorMessage = "حداقل یک کاربر باید انتخاب شود.")]
    public List<string> UserIds { get; set; } = new();
}
