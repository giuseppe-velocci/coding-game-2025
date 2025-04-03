using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using OrderApiGate;
using OrderApiGate.Orders;
using OrderApiGate.Orders.Service;
using Xunit;

namespace OrderApiGateTests
{
    public class OrderApiHandlerTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly OrderApiHandler _sut;

        public OrderApiHandlerTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            Mock<ILogger<OrderApiHandler>> loggerMock = new ();
            ApiGateConfig config = new()  { OrderApiEndpoint = "http://localhost" };

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _sut = new OrderApiHandler(_httpClientFactoryMock.Object, config, loggerMock.Object);
        }

        [Fact]
        public async Task Create_Should_Return_SuccessResult_When_Response_Is_Successful()
        {
            // Arrange
            var writeOrder = new WriteOrder { UserId = 1, AddressId = 1, ProductIds = new List<WriteOrderProduct>() };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"value\": 1}")
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _sut.Create(writeOrder, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(1, result.Value);
        }

        [Fact]
        public async Task Delete_Should_Return_SuccessResult_When_Response_Is_NoContent()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _sut.Delete(1, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task ReadAll_Should_Return_Orders_When_Response_Is_Successful()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[{\"OrderId\": 1, \"UserId\": 1, \"AddressId\": 1, \"OrderDate\": \"2023-01-01T00:00:00\"}]")
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _sut.ReadAll(CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_Order_When_Response_Is_Successful()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"OrderId\": 1, \"UserId\": 1, \"AddressId\": 1, \"OrderDate\": \"2023-01-01T00:00:00\"}")
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _sut.ReadOne(1, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(1, result.Value.OrderId);
        }

        [Fact]
        public async Task Update_Should_Return_SuccessResult_When_Response_Is_NoContent()
        {
            // Arrange
            var writeOrder = new WriteOrder { UserId = 1, AddressId = 1, ProductIds = new List<WriteOrderProduct>() };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _sut.Update(1, writeOrder, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
        }
    }
}
