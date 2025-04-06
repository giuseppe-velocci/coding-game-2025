using Core;
using Infrastructure;
using System.Text.Json;
using static ApiResponseHelper;

namespace OrderApiGate.Categories.Service
{
    public class CategoryApiHandler : AsbtractHttpApiClientService, ICrudHandler<Category, WriteCategory>
    {
        private readonly string _endpointUrl;
        private readonly static JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };

        public CategoryApiHandler(IHttpClientFactory httpClientFactory, ApiGateConfig config, ILogger<CategoryApiHandler> logger) :
            base(httpClientFactory, logger)
        {
            _endpointUrl = $"{config.ProductApiEndpoint}/categories";
        }

        public async Task<OperationResult<long>> Create(WriteCategory value, CancellationToken cts)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await _retryPolicy.ExecuteAsync(() =>
                    httpClient.PostAsJsonAsync(_endpointUrl, value, cts));

                var responseContent = await response.Content.ReadAsStringAsync(cts);
                if (response.IsSuccessStatusCode)
                {
                    var res = JsonSerializer.Deserialize<SerializableResult<long>>(responseContent, _serializerOptions);
                    return new SuccessResult<long>(res.Value);
                }
                else
                {
                    return GetOperationResultResponse<long>((int)response.StatusCode, responseContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during http call");
            }

            return new InvalidRequestResult<long>("Unexpected issues");
        }

        public async Task<OperationResult<None>> Delete(long id, CancellationToken cts)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await _retryPolicy.ExecuteAsync(() =>
                    httpClient.DeleteAsync($"{_endpointUrl}/{id}", cts));

                var responseContent = await response.Content.ReadAsStringAsync(cts);
                var statusCode = (int)response.StatusCode;
                if (statusCode == 204)
                {
                    return new SuccessResult<None>(None.Instance());
                }

                var intResponse = GetOperationResultResponse<string>(statusCode, responseContent);
                if (statusCode == 404)
                {
                    return new NotFoundResult<None>(intResponse.Message);
                }
                else
                {
                    return new ValidationFailureResult<None>(intResponse.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during http call");
            }

            return new InvalidRequestResult<None>("Unexpected issues");
        }

        public async Task<OperationResult<Category[]>> ReadAll(CancellationToken cts)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await _retryPolicy.ExecuteAsync(() =>
                httpClient.GetAsync(_endpointUrl, cts));

            var responseContent = await response.Content.ReadAsStringAsync(cts);
            var statusCode = (int)response.StatusCode;
            return GetOperationResultResponse<Category[]>(statusCode, responseContent)!;
        }

        public async Task<OperationResult<Category>> ReadOne(long id, CancellationToken cts)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await _retryPolicy.ExecuteAsync(() =>
                httpClient.GetAsync($"{_endpointUrl}/{id}", cts));

            var responseContent = await response.Content.ReadAsStringAsync(cts);
            var statusCode = (int)response.StatusCode;
            return GetOperationResultResponse<Category>(statusCode, responseContent)!;
        }

        public async Task<OperationResult<None>> Update(long id, WriteCategory value, CancellationToken cts)
        {
            try
            {
                Category add = new Category()
                {
                    CategoryId = id,
                    Name = value.Name,
                };
                var httpClient = _httpClientFactory.CreateClient();
                var response = await _retryPolicy.ExecuteAsync(() =>
                    httpClient.PutAsJsonAsync($"{_endpointUrl}/{id}", add, cts));

                var responseContent = await response.Content.ReadAsStringAsync(cts);
                var statusCode = (int)response.StatusCode;
                if (statusCode == 204)
                {
                    return new SuccessResult<None>(None.Instance());
                }

                var intResponse = GetOperationResultResponse<string>(statusCode, responseContent);
                if (statusCode == 404)
                {
                    return new NotFoundResult<None>(intResponse.Message);
                }
                else
                {
                    return new ValidationFailureResult<None>(intResponse.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during http call");
            }

            return new InvalidRequestResult<None>("Unexpected issues");
        }
    }
}
