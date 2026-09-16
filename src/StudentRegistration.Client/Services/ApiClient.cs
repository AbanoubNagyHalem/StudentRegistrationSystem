using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using StudentRegistration.Client.Models;

namespace StudentRegistration.Client.Services;


public class ApiClient
{
  private readonly HttpClient _http;
  private readonly ILocalStorageService _storage;

  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public ApiClient(HttpClient http, ILocalStorageService storage)
  {
    _http = http;
    _storage = storage;
  }

  public async Task<T?> GetAsync<T>(string path)
  {
    await AttachTokenAsync();
    var response = await _http.GetAsync(path);
    return await ReadAsync<T>(response);
  }

  public async Task<T?> PostAsync<T>(string path, object body)
  {
    await AttachTokenAsync();
    var response = await _http.PostAsJsonAsync(path, body);
    return await ReadAsync<T>(response);
  }

  public async Task<T?> PutAsync<T>(string path, object? body = null)
  {
    await AttachTokenAsync();
    var response = body is null
        ? await _http.PutAsync(path, null)
        : await _http.PutAsJsonAsync(path, body);
    return await ReadAsync<T>(response);
  }

  public async Task DeleteAsync(string path)
  {
    await AttachTokenAsync();
    var response = await _http.DeleteAsync(path);
    await EnsureSuccessAsync(response);
  }

  // ---------------------------------------------------------
  private async Task AttachTokenAsync()
  {
    var token = await _storage.GetItemAsync<string>("authToken");
    _http.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
        ? null
        : new AuthenticationHeaderValue("Bearer", token);
  }

  private static async Task<T?> ReadAsync<T>(HttpResponseMessage response)
  {
    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
      return default;

    var content = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
      throw new ApiException(ExtractError(content), (int)response.StatusCode);

    if (string.IsNullOrWhiteSpace(content))
      return default;

    return JsonSerializer.Deserialize<T>(content, JsonOptions);
  }

  private static async Task EnsureSuccessAsync(HttpResponseMessage response)
  {
    if (response.IsSuccessStatusCode) return;
    var content = await response.Content.ReadAsStringAsync();
    throw new ApiException(ExtractError(content), (int)response.StatusCode);
  }

  private static string ExtractError(string content)
  {
    try
    {
      var err = JsonSerializer.Deserialize<ApiError>(content, JsonOptions);
      return err?.Message ?? "Request failed.";
    }
    catch
    {
      return "Request failed.";
    }
  }
}

public class ApiException : Exception
{
  public int StatusCode { get; }
  public ApiException(string message, int statusCode) : base(message)
  {
    StatusCode = statusCode;
  }
}