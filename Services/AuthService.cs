using System.Net.Http.Json;
using Microsoft.JSInterop;
using MinimalHabitsBlazor.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace MinimalHabitsBlazor.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly AuthenticationStateProvider _authStateProvider;
    private const string TokenKey = "authToken";

    public AuthService(
        HttpClient httpClient, 
        IJSRuntime jsRuntime,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _authStateProvider = authStateProvider;
    }

    public async Task<string?> RegisterAsync(RegisterDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (result?.TryGetValue("token", out var token) == true)
        {
            await StoreTokenAsync(token);
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(token);
            return token;
        }

        return null;
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (result?.TryGetValue("token", out var token) == true)
        {
            await StoreTokenAsync(token);
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(token);
            return token;
        }

        return null;
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
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