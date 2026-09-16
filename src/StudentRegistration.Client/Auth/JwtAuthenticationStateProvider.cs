using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace StudentRegistration.Client.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
  private readonly ILocalStorageService _storage;

  public JwtAuthenticationStateProvider(ILocalStorageService storage)
  {
    _storage = storage;
  }

  public override async Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    var token = await _storage.GetItemAsync<string>("authToken");

    if (string.IsNullOrWhiteSpace(token))
      return Anonymous();

    var claims = ParseClaimsFromJwt(token);
    var expiry = claims.FirstOrDefault(c => c.Type == "exp")?.Value;

    if (expiry is not null &&
        long.TryParse(expiry, out var expUnix) &&
        DateTimeOffset.FromUnixTimeSeconds(expUnix) < DateTimeOffset.UtcNow)
    {
      await _storage.RemoveItemAsync("authToken");
      return Anonymous();
    }

    var identity = new ClaimsIdentity(claims, "jwt");
    return new AuthenticationState(new ClaimsPrincipal(identity));
  }

  public void NotifyAuthChanged() =>
      NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

  private static AuthenticationState Anonymous() =>
      new(new ClaimsPrincipal(new ClaimsIdentity()));

  private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
  {
    var payload = jwt.Split('.')[1];
    var jsonBytes = ParseBase64WithoutPadding(payload);
    var keyValues = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)!;

    return keyValues.Select(kv => new Claim(kv.Key, kv.Value?.ToString() ?? ""));
  }

  private static byte[] ParseBase64WithoutPadding(string base64)
  {
    switch (base64.Length % 4)
    {
      case 2: base64 += "=="; break;
      case 3: base64 += "="; break;
    }
    return Convert.FromBase64String(base64);
  }
}