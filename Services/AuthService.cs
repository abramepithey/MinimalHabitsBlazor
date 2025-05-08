using System.Net.Http.Json;
using Microsoft.JSInterop;
using MinimalHabitsBlazor.Models;

namespace MinimalHabitsBlazor.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "authToken";

    public event Action<string?>? AuthenticationChanged;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> RegisterAsync(RegisterDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Registration failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (result?.TryGetValue("token", out var token) == true)
            {
                await StoreTokenAsync(token);
                AuthenticationChanged?.Invoke(token);
                return token;
            }

            throw new InvalidOperationException("Token not found in response");
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to connect to the API server. Please ensure the API server is running at {_httpClient.BaseAddress}", ex);
        }
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Login failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (result?.TryGetValue("token", out var token) == true)
            {
                await StoreTokenAsync(token);
                AuthenticationChanged?.Invoke(token);
                return token;
            }

            throw new InvalidOperationException("Token not found in response");
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to connect to the API server. Please ensure the API server is running at {_httpClient.BaseAddress}", ex);
        }
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        AuthenticationChanged?.Invoke(null);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
    }

    private async Task StoreTokenAsync(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
    }
} 