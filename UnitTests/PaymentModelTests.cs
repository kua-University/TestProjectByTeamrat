using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stripe.Checkout;
using ContosoUniversity.Pages;
using ContosoUniversity.Services;
using Microsoft.Extensions.Configuration;

namespace DemoUniversity.TestssMs.UnitTests
{
    [TestClass]
    public class PaymentModelTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IStripeSessionService> _mockStripeSessionService;

        [TestInitialize]
        public void Setup()
        {
            // Mock IConfiguration to return a test publishable key
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["Stripe:PublishableKey"]).Returns("test_publishable_key");

            // Mock IStripeSessionService to simulate session creation
            _mockStripeSessionService = new Mock<IStripeSessionService>();
            _mockStripeSessionService.Setup(service => service.Create(It.IsAny<SessionCreateOptions>()))
                .Returns(new Session { Id = "test_session_id" });
        }

        [TestMethod]
        public void OnGet_ShouldSetPublishableKeyFromConfiguration()
        {
            // Arrange
            var paymentModel = new PaymentModel(_mockConfiguration.Object, _mockStripeSessionService.Object);

            // Act
            paymentModel.OnGet();

            // Assert
            Assert.AreEqual("test_publishable_key", paymentModel.PublishableKey);
        }

        [TestMethod]
        public void OnGet_ShouldCreateCheckoutSessionWithCorrectOptions()
        {
            // Arrange
            var paymentModel = new PaymentModel(_mockConfiguration.Object, _mockStripeSessionService.Object);

            // Act
            paymentModel.OnGet();

            // Assert
            _mockStripeSessionService.Verify(service => service.Create(It.Is<SessionCreateOptions>(options =>
                options.PaymentMethodTypes.Contains("card") &&
                options.LineItems.Count == 1 &&
                options.LineItems[0].PriceData.UnitAmount == 5000 &&
                options.LineItems[0].PriceData.Currency == "usd" &&
                options.LineItems[0].PriceData.ProductData.Name == "Contoso University Payment" &&
                options.Mode == "payment" &&
                options.SuccessUrl == "https://localhost:55275/Students/Success" &&
                options.CancelUrl == "https://localhost:55275/Students/Cancel"
            )), Times.Once);
        }

        [TestMethod]
        public void OnGet_ShouldSetCheckoutSessionIdFromCreatedSession()
        {
            // Arrange
            var paymentModel = new PaymentModel(_mockConfiguration.Object, _mockStripeSessionService.Object);

            // Act
            paymentModel.OnGet();

            // Assert
            Assert.AreEqual("test_session_id", paymentModel.CheckoutSessionId);
        }
    }
}