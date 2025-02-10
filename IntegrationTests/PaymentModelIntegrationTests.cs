using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stripe.Checkout;
using ContosoUniversity.Pages;
using ContosoUniversity.Services;

namespace DemoUniversity.TestssMs.IntegrationTests
{
    [TestClass]
    public class PaymentModelIntegrationTests
    {
        private IConfiguration _configuration;
        private IStripeSessionService _stripeSessionService;

        [TestInitialize]
        public void Setup()
        {
            // Mock IConfiguration to return a test publishable key
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(config => config["Stripe:PublishableKey"]).Returns("test_publishable_key");
            _configuration = mockConfiguration.Object;

            // Set the Stripe API key for testing
            Stripe.StripeConfiguration.ApiKey = "sk_test_51QaBOqDta4PXtgUsAuJiSpleS5mvf4iYFRLO7tjbKq6J17tjYnW6V3GoQaV9ie5pSGb8yxzUDyw2LmnReCNuLV5G00pqrbXNsM";

            // Use a real implementation of IStripeSessionService
            _stripeSessionService = new StripeSessionService();
        }

        [TestMethod]
        public void OnGet_ShouldSetPublishableKeyFromConfiguration()
        {
            // Arrange
            var paymentModel = new PaymentModel(_configuration, _stripeSessionService);

            // Act
            paymentModel.OnGet();

            // Assert
            Assert.AreEqual("test_publishable_key", paymentModel.PublishableKey);
        }

        [TestMethod]
        public void OnGet_ShouldCreateCheckoutSessionWithCorrectOptions()
        {
            // Arrange
            var paymentModel = new PaymentModel(_configuration, _stripeSessionService);

            // Act
            paymentModel.OnGet();

            // Assert
            Assert.IsNotNull(paymentModel.CheckoutSessionId);
            Assert.IsTrue(paymentModel.CheckoutSessionId.StartsWith("cs_test_")); // Example Stripe session ID format
        }
    }
}