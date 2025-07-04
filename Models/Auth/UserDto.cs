﻿namespace BlazorApp1.Models;

public class UserDto
{
    public string Id { get; set; } = "";
    public string UserName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Role { get; set; } = "";
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
