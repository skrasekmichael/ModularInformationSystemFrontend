using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using RailwayResult;

using TeamUp.Contracts;
using TeamUp.DAL.Api.Internal;

namespace TeamUp.DAL.Api;

public sealed partial class ApiClient
{
	private readonly HttpClient _client;
	private readonly IAuthService _authService;

	public ApiClient(HttpClient client, IAuthService authService)
	{
		_client = client;
		_authService = authService;
	}

	private Task<Result> SendAsync(HttpMethod method, string uri, CancellationToken ct) => SendAsync(method, uri, null, ct);

	private Task<Result> SendAsync(HttpMethod method, string uri, Func<HttpRequestMessage, bool>? configure, CancellationToken ct)
	{
		var request = new HttpRequestMessage
		{
			Method = method,
			RequestUri = new Uri(uri, UriKind.Relative),
		};

		var redirectOnUnAuthorized = true;
		if (configure is not null)
		{
			redirectOnUnAuthorized = configure(request);
		}

		return SendRequestAsync(request, redirectOnUnAuthorized, ct);
	}

	private Task<Result<TResponse>> SendAsync<TResponse>(HttpMethod method, string uri, CancellationToken ct) =>
		SendAsync<TResponse>(method, uri, null, ct);

	private Task<Result<TResponse>> SendAsync<TResponse>(HttpMethod method, string uri, Func<HttpRequestMessage, bool>? configure, CancellationToken ct)
	{
		var request = new HttpRequestMessage
		{
			Method = method,
			RequestUri = new Uri(uri, UriKind.Relative),
		};

		var redirectOnUnAuthorized = true;
		if (configure is not null)
		{
			redirectOnUnAuthorized = configure(request);
		}

		return SendRequestAsync<TResponse>(request, redirectOnUnAuthorized, ct);
	}

	private Task<Result<TResponse>> SendAsync<TRequest, TResponse>(HttpMethod method, string uri, TRequest payload, CancellationToken ct) =>
		SendAsync<TRequest, TResponse>(method, uri, payload, null, ct);

	private async Task<Result<TResponse>> SendAsync<TRequest, TResponse>(HttpMethod method, string uri, TRequest? payload, Func<HttpRequestMessage, bool>? configure, CancellationToken ct)
	{
		var json = JsonSerializer.Serialize(payload);
		var request = new HttpRequestMessage
		{
			Method = method,
			RequestUri = new Uri(uri, UriKind.Relative),
			Content = new StringContent(json, Encoding.UTF8, "application/json"),
		};

		var redirectOnUnAuthorized = true;
		if (configure is not null)
		{
			redirectOnUnAuthorized = configure(request);
		}

		return await SendRequestAsync<TResponse>(request, redirectOnUnAuthorized, ct);
	}

	private Task<Result> SendAsync<TRequest>(HttpMethod method, string uri, TRequest payload, CancellationToken ct) =>
		SendAsync(method, uri, payload, null, ct);

	private async Task<Result> SendAsync<TRequest>(HttpMethod method, string uri, TRequest? payload, Func<HttpRequestMessage, bool>? configure, CancellationToken ct)
	{
		var json = JsonSerializer.Serialize(payload);
		var request = new HttpRequestMessage
		{
			Method = method,
			RequestUri = new Uri(uri, UriKind.Relative),
			Content = new StringContent(json, Encoding.UTF8, "application/json"),
		};

		var redirectOnUnAuthorized = true;
		if (configure is not null)
		{
			redirectOnUnAuthorized = configure(request);
		}

		return await SendRequestAsync(request, redirectOnUnAuthorized, ct);
	}

	private async Task<Result> SendRequestAsync(HttpRequestMessage request, bool redirectOnUnAuthorized, CancellationToken ct)
	{
		await InjectAuthToken(request, ct);

		HttpResponseMessage response;

		try
		{
			response = await _client.SendAsync(request, ct);
		}
		catch (Exception ex)
		{
			return new ApiUnexpectedError("Api.UnexpectedError", ex.Message);
		}

		if (!response.IsSuccessStatusCode)
		{
			return await ParseErrorResponse(response, redirectOnUnAuthorized, ct);
		}

		return Result.Success;
	}

	private async Task<Result<TResponse>> SendRequestAsync<TResponse>(HttpRequestMessage request, bool redirectOnUnAuthorized, CancellationToken ct)
	{
		await InjectAuthToken(request, ct);

		HttpResponseMessage response;

		try
		{
			response = await _client.SendAsync(request, ct);
		}
		catch (Exception ex)
		{
			return new ApiUnexpectedError("Api.UnexpectedError", ex.Message);
		}

		if (!response.IsSuccessStatusCode)
		{
			return await ParseErrorResponse(response, redirectOnUnAuthorized, ct);
		}

		var responsePayload = await response.Content.ReadFromJsonAsync<TResponse>(ct);
		if (responsePayload is null)
		{
			return new ApiError("Api.SerializationError", "Failed to deserialize response.", response.StatusCode);
		}

		return responsePayload;
	}

	private async Task<Error> ParseErrorResponse(HttpResponseMessage response, bool redirectOnUnAuthorized, CancellationToken ct)
	{
		if (redirectOnUnAuthorized && response.StatusCode == HttpStatusCode.Unauthorized)
		{
			await _authService.LogoutAsync(ct: ct);
		}

		var contentType = response.Content.Headers.ContentType?.MediaType;
		if (contentType != "application/problem+json")
		{
			return new ApiError("Api.UnexpectedError", $"Unexpected response with content type '{contentType}'.", response.StatusCode);
		}

		var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(ct);
		if (problemDetails is null)
		{
			return new ApiError("Api.SerializationError", "Failed to deserialize error response.", response.StatusCode);
		}

		var title = problemDetails.Title ?? "Undefined Title";
		var detail = problemDetails.Detail ?? "Undefined Detail";

		if (problemDetails.Errors.Count > 0)
		{
			return new ApiValidationError("Api.ValidationError", "One or more validation errors occurred.", problemDetails.Errors);
		}

		return new ApiError("Api.Error", detail, response.StatusCode);
	}

	private async Task InjectAuthToken(HttpRequestMessage request, CancellationToken ct)
	{
		var jwt = await _authService.GetTokenAsync(ct);
		if (jwt is not null)
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
		}
	}
}
