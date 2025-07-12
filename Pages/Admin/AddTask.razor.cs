using Microsoft.AspNetCore.Components;
using MudBlazor;
using BlazorApp1.Models.TaskManagement;

namespace BlazorApp1.Components.Admin
{
    public partial class AddTask : ComponentBase
    {
        [Parameter] public List<SimpleUserDto> Users { get; set; } = new();
        [Parameter] public bool IsEditing { get; set; }
        [Parameter]
        public string? Mode
        {
            get => IsEditing ? "edit" : "add";
            set => IsEditing = string.Equals(value, "edit", StringComparison.OrdinalIgnoreCase);
        }
        [Parameter] public CreateTaskDto? Model { get; set; }
        [Parameter] public CreateTaskDto? InitialData { get; set; }
        [Parameter] public EventCallback<CreateTaskDto> OnSubmit { get; set; }
        [Parameter] public EventCallback<CreateTaskDto> OnSaved { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }
        [Parameter] public bool Loading { get; set; }

        protected CreateTaskDto _model = new();
        protected string _selectedUserId = string.Empty;
        protected DateTime? _deadline;
        protected MudForm? form;
        protected bool isValid;
        protected string[] errors = { };

        protected override void OnParametersSet()
        {
            if (Model is not null) _model = Clone(Model);
            else if (InitialData is not null) _model = Clone(InitialData);
            else if (string.IsNullOrWhiteSpace(_model.Title))
                _model = new() { Deadline = DateTime.Today };

            _selectedUserId = _model.UserIds?.FirstOrDefault() ?? string.Empty;
            _deadline = _model.Deadline == default
                ? DateTime.Today
                : _model.Deadline;
        }

        protected async Task SaveClicked()
        {
            await form!.Validate();
            if (!form.IsValid) return;

            _model.UserIds = new() { _selectedUserId };
            _model.Deadline = _deadline ?? DateTime.Today;

            if (OnSubmit.HasDelegate)
                await OnSubmit.InvokeAsync(_model);
            else if (OnSaved.HasDelegate)
                await OnSaved.InvokeAsync(_model);
        }

        protected async Task CancelClicked()
        {
            if (OnCancel.HasDelegate)
                await OnCancel.InvokeAsync();
        }

        protected IEnumerable<string> ValidateDeadline(DateTime? date)
        {
            if (date is null) yield break;
            if (date.Value.Date < DateTime.Today)
                yield return "تاریخ نمی‌تواند قبل از امروز باشد!";
        }

        protected static CreateTaskDto Clone(CreateTaskDto src) => new()
        {
            Title = src.Title,
            Description = src.Description,
            Deadline = src.Deadline,
            UserIds = new(src.UserIds ?? new List<string>())
        };
    }
}
