using BlazorApp1.Models;

namespace BlazorApp1.Services.State;

public class ActivityState
{
    public ActivityDto? SelectedActivity { get; private set; }

    public void SetActivity(ActivityDto activity)
    {
        SelectedActivity = activity;
    }

    public void Clear()
    {
        SelectedActivity = null;
    }
}
