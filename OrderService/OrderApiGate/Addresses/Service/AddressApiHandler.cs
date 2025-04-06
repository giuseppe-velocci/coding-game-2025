using Core;
using Infrastructure;
using static ApiResponseHelper;

namespace OrderApiGate.Addresses.Service
{
    public class AddressApiHandler : AsbtractHttpApiClientService, ICrudHandler<Address, WriteAddress>
    {
        private readonly string _endpointUrl;

        public AddressApiHandler(IHttpClientFactory httpClientFactory, ApiGateConfig config, ILogger<AddressApiHandler> logger) :
            base(httpClientFactory, logger)
        {
            _endpointUrl = $"{config.AddressApiEndpoint}/addresses";
        }

        public async Task<OperationResult<long>> Create(WriteAddress value, CancellationToken cts)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await _retryPolicy.ExecuteAsync(() =>
                    httpClient.PostAsJsonAsync(_endpointUrl, value, cts));

                var responseContent = await response.Content.ReadAsStringAsync(cts);
                return GetOperationResultResponse<long>((int)response.StatusCode, responseContent);
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

        public async Task<OperationResult<Address[]>> ReadAll(CancellationToken cts)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await _retryPolicy.ExecuteAsync(() =>
                httpClient.GetAsync(_endpointUrl, cts));

            var responseContent = await response.Content.ReadAsStringAsync(cts);
            var statusCode = (int)response.StatusCode;
            return GetOperationResultResponse<Address[]>(statusCode, responseContent)!;
        }

        public async Task<OperationResult<Address>> ReadOne(long id, CancellationToken cts)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await _retryPolicy.ExecuteAsync(() =>
                httpClient.GetAsync($"{_endpointUrl}/{id}", cts));

            var responseContent = await response.Content.ReadAsStringAsync(cts);
            var statusCode = (int)response.StatusCode;
            return GetOperationResultResponse<Address>(statusCode, responseContent)!;
        }

        public async Task<OperationResult<None>> Update(long id, WriteAddress value, CancellationToken cts)
        {
            try
            {
                Address add = new Address()
                {
                    AddressId = id,
                    Street = value.Street,
                    City = value.City,
                    State = value.State,
                    Country = value.Country,
                    ZipCode = value.ZipCode,
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
