using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using StudentRegistration.Client.Auth;
using StudentRegistration.Client.Models;

namespace StudentRegistration.Client.Services;

public class AuthService
{
  private readonly ApiClient _api;
  private readonly ILocalStorageService _storage;
  private readonly JwtAuthenticationStateProvider _authState;

  public AuthService(ApiClient api, ILocalStorageService storage, JwtAuthenticationStateProvider authState)
  {
    _api = api;
    _storage = storage;
    _authState = authState;
  }

  public async Task<LoginResponse> StudentLoginAsync(string regNumber, string pin)
  {
    var result = await _api.PostAsync<LoginResponse>("/api/auth/login",
        new { registrationNumber = regNumber, pin });

    if (result is null || string.IsNullOrWhiteSpace(result.Token))
      throw new InvalidOperationException("Login failed.");

    await _storage.SetItemAsync("authToken", result.Token);
    await _storage.SetItemAsync("userInfo", result);
    _authState.NotifyAuthChanged();

    return result;
  }

  public async Task<LoginResponse> AdminLoginAsync(string username, string password)
  {
    var result = await _api.PostAsync<LoginResponse>("/api/auth/admin-login",
        new { username, password });

    if (result is null || string.IsNullOrWhiteSpace(result.Token))
      throw new InvalidOperationException("Login failed.");

    await _storage.SetItemAsync("authToken", result.Token);
    await _storage.SetItemAsync("userInfo", result);
    _authState.NotifyAuthChanged();

    return result;
  }

  public async Task<LoginResponse?> GetUserInfoAsync() =>
      await _storage.GetItemAsync<LoginResponse>("userInfo");

  public async Task LogoutAsync()
  {
    await _storage.RemoveItemAsync("authToken");
    await _storage.RemoveItemAsync("userInfo");
    _authState.NotifyAuthChanged();
  }

  public async Task<bool> IsAuthenticatedAsync() =>
      !string.IsNullOrWhiteSpace(await _storage.GetItemAsync<string>("authToken"));
}