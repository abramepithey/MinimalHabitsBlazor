using System.Net.Http.Json;
using System.Net.Http.Headers;
using MinimalHabitsBlazor.Models;

namespace MinimalHabitsBlazor.Services;

public class HabitService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public HabitService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private async Task SetAuthHeader()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    // Habits CRUD
    public async Task<List<Habit>> GetHabitsAsync()
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<Habit>>("api/habits") ?? new List<Habit>();
    }

    public async Task<Habit?> GetHabitAsync(int id)
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<Habit>($"api/habits/{id}");
    }

    public async Task<Habit?> CreateHabitAsync(HabitDto dto)
    {
        await SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/habits", dto);
        return response.IsSuccessStatusCode 
            ? await response.Content.ReadFromJsonAsync<Habit>() 
            : null;
    }

    public async Task<bool> UpdateHabitAsync(int id, HabitDto dto)
    {
        await SetAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"api/habits/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteHabitAsync(int id)
    {
        await SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"api/habits/{id}");
        return response.IsSuccessStatusCode;
    }

    // Habit Entries CRUD
    public async Task<List<HabitEntry>> GetHabitEntriesAsync(int habitId)
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<HabitEntry>>($"api/habits/{habitId}/entries") ?? new List<HabitEntry>();
    }

    public async Task<HabitEntry?> GetHabitEntryAsync(int habitId, int entryId)
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<HabitEntry>($"api/habits/{habitId}/entries/{entryId}");
    }

    public async Task<HabitEntry?> CreateHabitEntryAsync(int habitId, HabitEntryDto dto)
    {
        await SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync($"api/habits/{habitId}/entries", dto);
        return response.IsSuccessStatusCode 
            ? await response.Content.ReadFromJsonAsync<HabitEntry>() 
            : null;
    }

    public async Task<bool> UpdateHabitEntryAsync(int habitId, int entryId, HabitEntryDto dto)
    {
        await SetAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"api/habits/{habitId}/entries/{entryId}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteHabitEntryAsync(int habitId, int entryId)
    {
        await SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"api/habits/{habitId}/entries/{entryId}");
        return response.IsSuccessStatusCode;
    }
} 