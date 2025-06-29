using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace BlazorApp1.Services;

public class SignalRService
{
    private HubConnection? _connection;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public async Task StartConnectionAsync(string token)
    {
        if (_connection is { State: HubConnectionState.Connected or HubConnectionState.Connecting })
            return;

        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5001/hubs/task", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect()
            .Build();

        await _connection.StartAsync();
    }

    public async Task StopConnectionAsync()
    {
        if (_connection is not null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

    public void OnTaskAssigned(Action<int> callback)
    {
        _connection?.On("TaskAssigned", callback);
    }

    public void OnTaskCompletedByUser(Action<int, int?> callback)
    {
        _connection?.On<object>("TaskCompletedByUser", payload =>
        {
            try
            {
                var json = payload?.ToString();
                var data = JsonSerializer.Deserialize<TaskCompletionPayload>(json!);
                if (data != null)
                    callback(data.UserTaskId, data.Percent);
            }
            catch
            {
                callback(0, null);
            }
        });
    }

    public void OnTaskConfirmed(Action<int> callback) =>
        _connection?.On("TaskConfirmed", callback);

    public void OnTaskUpdated(Action<int> callback) =>
        _connection?.On("TaskUpdated", callback);

    public void OnTaskDeleted(Action<int> callback) =>
        _connection?.On("TaskDeleted", callback);

    public void RemoveAllListeners()
    {
        if (_connection == null) return;
        _connection.Remove("TaskAssigned");
        _connection.Remove("TaskCompletedByUser");
        _connection.Remove("TaskConfirmed");
        _connection.Remove("TaskUpdated");
        _connection.Remove("TaskDeleted");
    }

    public async Task SendTaskAssigned(string userId, int taskId) =>
        await _connection?.InvokeAsync("SendTaskAssignedMessage", userId, taskId)!;

    public async Task SendTaskCompleted(string adminId, int userTaskId) =>
        await _connection?.InvokeAsync("SendTaskCompletedMessage", adminId, userTaskId)!;

    public async Task SendTaskConfirmed(string userId, int userTaskId) =>
        await _connection?.InvokeAsync("SendTaskConfirmedMessage", userId, userTaskId)!;

    public async Task SendTaskUpdated(string userId, int taskId) =>
        await _connection?.InvokeAsync("SendTaskUpdatedMessage", userId, taskId)!;

    public async Task SendTaskDeleted(string userId, int taskId) =>
        await _connection?.InvokeAsync("SendTaskDeletedMessage", userId, taskId)!;
}

// payload برای TaskCompletedByUser
public class TaskCompletionPayload
{
    public int UserTaskId { get; set; }
    public int? Percent { get; set; }
}
