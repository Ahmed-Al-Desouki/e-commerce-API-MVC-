using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using FluentAssertions;
using Moq;

namespace ECommerce.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IPaymentGateway> _paymentGatweyMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly Mock<IMerchantOrderIdGenerator> _merchantOrderIdGeneratorMock;
        private readonly Mock<IPaymentFlowSettings> _paymentFlowSettingsMock;
        private readonly OrderService _orderService;
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<ICartRepository> _cartRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;

        public OrderServiceTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _cartRepoMock = new Mock<ICartRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _paymentGatweyMock = new Mock<IPaymentGateway>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _merchantOrderIdGeneratorMock = new Mock<IMerchantOrderIdGenerator>();
            _paymentFlowSettingsMock = new Mock<IPaymentFlowSettings>();
            _orderService = new OrderService(_orderRepoMock.Object, _cartRepoMock.Object, _productRepoMock.Object, _userRepoMock.Object, _paymentGatweyMock.Object, _dateTimeProviderMock.Object, _merchantOrderIdGeneratorMock.Object, _paymentFlowSettingsMock.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetUserOrdersAsync_ArgumentOutOfRangeException_userIdLessThanOrEqualZero(int userId)
        {
            // Arrenge

            // Act
            var actual = async () => await _orderService.GetUserOrdersAsync(userId);

            // Assert
            await actual.Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async Task GetUserOrdersAsync_ReturnOrderDto_ExpireOrderAndRestoreStock()
        {
            // Arrenge
            int userId = 1;

            var order = new Order
            {
                UserId = 1,

                TotalAmount = 200,

                Status = OrderStatus.PendingPayment,

                BillingFirstName = "Mohamed",
                BillingLastName = "Ahmed",
                BillingEmail = "amohamedahmad6@.com",
                BillingPhoneNumber = "01157126303",
                BillingStreet = "nasser",
                BillingBuilding = "10",
                BillingCity = "Cairo",
                BillingCountry = "EG",
                BillingState = "Cairo",
                BillingApartment = "1",
                BillingFloor = "2",

                Items = new List<OrderItem>()
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        Price = 10
                    },
                    new OrderItem
                    {
                        ProductId = 2,
                        Quantity = 1,
                        Price = 10
                    }
                }
            };

            var orders = new List<Order>();
            orders.Add(order);

            _orderRepoMock.Setup(i => i.GetByUserIdAsync(userId)).ReturnsAsync(orders);

            var now = new DateTime(2026, 5, 8, 3, 44, 0, DateTimeKind.Utc);
            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            var expiredOrder = new Order
            {
                UserId = 1,
                TotalAmount = 200,
                Status = OrderStatus.PendingPayment,

                BillingFirstName = "Mohamed",
                BillingLastName = "Ahmed",
                BillingEmail = "amohamedahmad6@.com",
                BillingPhoneNumber = "01157126303",
                BillingStreet = "nasser",
                BillingBuilding = "10",
                BillingCity = "Cairo",

 
                Payment = new Payment
                {
                    Status = PaymentStatus.Pending,
                    ExpiresAt = now.AddMinutes(-10) 
                },

                Items = new List<OrderItem>()
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        Price = 10
                    }
                }
            };

            orders.Add(expiredOrder);

            // Act
            var actual = await _orderService.GetUserOrdersAsync(userId);

            // Assert
            actual.Should().NotBeNull();
            _orderRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            expiredOrder.Status.Should().Be(OrderStatus.PaymentExpired);
            expiredOrder.Payment!.Status.Should().Be(PaymentStatus.Expired);
            expiredOrder.Payment.FailureReason.Should().Be("Payment session expired.");
            expiredOrder.Payment.CompletedAt.Should().Be(now);
        }


        [Fact]
        public async Task InitiateCheckoutAsync_ReturnCheckoutSessionDto_WhenDataIsCorrect()
        {
            // Arrange
            int userId = 1;

            var now = new DateTime(2026, 5, 8, 3, 44, 0, DateTimeKind.Utc);

            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            _merchantOrderIdGeneratorMock.Setup(x => x.Generate()).Returns("ECOM-hgh5445");

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 10,
                Stock = 10
            };

            var cart = new Cart
            {
                UserId = userId,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                }
            };

            var user = new User
            {
                Id = userId,
                Email = "amohamed@gmail.com"
            };

            var payGetSessionDto = new PaymentGatewaySessionDto
            {
                ProviderOrderId = "gg",
                PaymentToken = "ggg123",
                CheckoutUrl = "http://localhost:5000/home?localPayment=success&orderId=100&paymentStatus=Succeeded&orderStatus=Paid&totalAmount=20.00"
            };

            var order = new Order
            {
                Id = 10,
                UserId = 1,

                TotalAmount = 200,

                Status = OrderStatus.PendingPayment,

                BillingFirstName = "Mohamed",
                BillingLastName = "Ahmed",
                BillingEmail = "amohamedahmad6@.com",
                BillingPhoneNumber = "01157126303",
                BillingStreet = "nasser",
                BillingBuilding = "10",
                BillingCity = "Cairo",
                BillingCountry = "EG",
                BillingState = "Cairo",
                BillingApartment = "1",
                BillingFloor = "2",

                Items = new List<OrderItem>()
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        Price = 10
                    },
                    new OrderItem
                    {
                        ProductId = 2,
                        Quantity = 1,
                        Price = 10
                    }
                }
            };

            var orders = new List<Order>();
            orders.Add(order);


            _orderRepoMock.Setup(i => i.GetByUserIdAsync(userId)).ReturnsAsync(orders);
            _cartRepoMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(cart);
            _userRepoMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _productRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
            _paymentGatweyMock.Setup(x => x.CreateCardPaymentAsync(It.IsAny<PaymentGatewayRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payGetSessionDto);
            _paymentFlowSettingsMock.Setup(x => x.EnableLocalBypass).Returns(true);

            _orderRepoMock.Setup(x => x.AddAsync(It.IsAny<Order>()))
                        .Callback<Order>(o =>
                        {
                            o.Id = 100;
                            o.Payment!.Id = 200;
        
                        }).Returns(Task.CompletedTask);

            var checkoutRequestDto = new CheckoutRequestDto
            {
                FirstName = "Mohamed",
                LastName = "Ahmed",
                Email = "amohamed@gmail.com",
                PhoneNumber = "01111111111",
                Street = "Nasr",
                Building = "10",
                City = "Cairo",
                Country = "EG",
                State = "Cairo",
                ReturnUrl = "http://localhost:5000/home"
            };

            // Act
            var actual = await _orderService.InitiateCheckoutAsync(userId, checkoutRequestDto);

            // Assert
            actual.Should().NotBeNull();
            actual.OrderId.Should().Be(100);
            actual.PaymentId.Should().Be(200);
            actual.CheckoutUrl.Should().Be("http://localhost:5000/home?localPayment=success&orderId=100&paymentStatus=Succeeded&orderStatus=Paid&totalAmount=20.00");
            actual.ExpiresAt.Should().Be(now);

            _orderRepoMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
            _orderRepoMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
            _paymentGatweyMock.Verify(x => x.CreateCardPaymentAsync(It.IsAny<PaymentGatewayRequestDto>(),It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task InitiateCheckoutAsync_ReturnCheckoutSessionDto_WhenDataIsCorrectWithPaymob()
        {
            // Arrange
            int userId = 1;

            var now = new DateTime(2026, 5, 8, 3, 44, 0, DateTimeKind.Utc);

            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            _merchantOrderIdGeneratorMock.Setup(x => x.Generate()).Returns("ECOM-hgh5445");

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 10,
                Stock = 10
            };

            var cart = new Cart
            {
                UserId = userId,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Quantity = 2
                    }
                }
            };

            var user = new User
            {
                Id = userId,
                Email = "amohamed@gmail.com"
            };

            var payGetSessionDto = new PaymentGatewaySessionDto
            {
                ProviderOrderId = "gg",
                PaymentToken = "ggg123",
                CheckoutUrl = "https://paymob.com/checkout/session/123"
            };

            var order = new Order
            {
                Id = 10,
                UserId = 1,

                TotalAmount = 200,

                Status = OrderStatus.PendingPayment,

                BillingFirstName = "Mohamed",
                BillingLastName = "Ahmed",
                BillingEmail = "amohamedahmad6@.com",
                BillingPhoneNumber = "01157126303",
                BillingStreet = "nasser",
                BillingBuilding = "10",
                BillingCity = "Cairo",
                BillingCountry = "EG",
                BillingState = "Cairo",
                BillingApartment = "1",
                BillingFloor = "2",

                Items = new List<OrderItem>()
                {
                    new OrderItem
                    {
                        ProductId = 1,
                        Quantity = 2,
                        Price = 10
                    },
                    new OrderItem
                    {
                        ProductId = 2,
                        Quantity = 1,
                        Price = 10
                    }
                }
            };

            var orders = new List<Order>();
            orders.Add(order);


            _orderRepoMock.Setup(i => i.GetByUserIdAsync(userId)).ReturnsAsync(orders);
            _cartRepoMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(cart);
            _userRepoMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _productRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
            _paymentGatweyMock.Setup(x => x.CreateCardPaymentAsync(It.IsAny<PaymentGatewayRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payGetSessionDto);

            _orderRepoMock.Setup(x => x.AddAsync(It.IsAny<Order>()))
                        .Callback<Order>(o =>
                        {
                            o.Id = 100;
                            o.Payment!.Id = 200;

                        }).Returns(Task.CompletedTask);

            var checkoutRequestDto = new CheckoutRequestDto
            {
                FirstName = "Mohamed",
                LastName = "Ahmed",
                Email = "amohamed@gmail.com",
                PhoneNumber = "01111111111",
                Street = "Nasr",
                Building = "10",
                City = "Cairo",
                Country = "EG",
                State = "Cairo",
                ReturnUrl = "https://rujta.com/home"
            };

            // Act
            var actual = await _orderService.InitiateCheckoutAsync(userId, checkoutRequestDto);

            // Assert
            actual.Should().NotBeNull();
            actual.OrderId.Should().Be(100);
            actual.PaymentId.Should().Be(200);
            actual.CheckoutUrl.Should().Be("https://paymob.com/checkout/session/123");
            actual.ExpiresAt.Should().Be(now.AddMinutes(60));

            _orderRepoMock.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
            _orderRepoMock.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
            _paymentGatweyMock.Verify(x => x.CreateCardPaymentAsync(It.IsAny<PaymentGatewayRequestDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InitiateCheckoutAsync_ShouldThrowException_WhenEmailIsEmpty()
        {
            // Arrange
            int userId = 1;

            var checkoutRequestDto = new CheckoutRequestDto
            {
                FirstName = "Mohamed",
                LastName = "Ahmed",
                Email = "",
                PhoneNumber = "01111111111",
                Street = "Nasr",
                Building = "10",
                City = "Cairo",
                Country = "EG",
                State = "Cairo",
                ReturnUrl = "https://rujta.com/home"
            };

            // Act
            var actual = async () => await _orderService.InitiateCheckoutAsync(userId, checkoutRequestDto);

            // Assert
            await actual.Should().ThrowAsync<InvalidOperationException>();
            _orderRepoMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ProcessPaymentCallbackAsync_ArgumentNullException_WhenCallbackIsNull()
        {
            // Arrange

            // Act
            var actual = async () => await _orderService.ProcessPaymentCallbackAsync((PaymentCallbackDto)null!);

            // Assert
            await actual.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task ProcessPaymentCallbackAsync_InvalidOperationException_WhenIsNotCallbackAuthentic()
        {
            // Arrange
            var callback = new PaymentCallbackDto
            {
                MerchantOrderId = "ECOM-HarryPotter8"
            };

            _paymentGatweyMock.Setup(x => x.IsCallbackAuthentic(callback)).Returns(false);

            // Act
            var actual = async () => await _orderService.ProcessPaymentCallbackAsync(callback);

            // Assert
            await actual.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ProcessPaymentCallbackAsync_InvalidOperationException_WhenMerchantOrderIdIsNull()
        {
            // Arrange
            var callback = new PaymentCallbackDto
            {
                MerchantOrderId = null
            };

            _paymentGatweyMock.Setup(x => x.IsCallbackAuthentic(callback)).Returns(true);

            // Act
            var actual = async () => await _orderService.ProcessPaymentCallbackAsync(callback);

            // Assert
            await actual.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ProcessPaymentCallbackAsync_InvalidOperationException_WhenOrderIsNull()
        {
            // Arrange
            var callback = new PaymentCallbackDto
            {
                MerchantOrderId = "ECOM-123"
            };

            _paymentGatweyMock.Setup(x => x.IsCallbackAuthentic(callback)).Returns(true);

            _orderRepoMock.Setup(x => x.GetByMerchantOrderIdAsync("ECOM-123")).ReturnsAsync((Order?)null);

            // Act
            var actual = async () => await _orderService.ProcessPaymentCallbackAsync(callback);

            // Assert
            await actual.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ProcessPaymentCallbackAsync_InvalidOperationException_WhenPaymentIsNull()
        {
            // Arrange
            var callback = new PaymentCallbackDto
            {
                MerchantOrderId = "ECOM-123"
            };

            var order = new Order
            {
                Payment = null
            };

            _paymentGatweyMock.Setup(x => x.IsCallbackAuthentic(callback)).Returns(true);

            _orderRepoMock.Setup(x => x.GetByMerchantOrderIdAsync("ECOM-123")).ReturnsAsync(order);

            // Act
            var actual = async () => await _orderService.ProcessPaymentCallbackAsync(callback);

            // Assert
            await actual.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ProcessPaymentCallbackAsync_ShouldReturnSuccess_WhenPaymentIsCorrect()
        {
            // Arrange
            var callback = new PaymentCallbackDto
            {
                MerchantOrderId = "ECOM-123",
                Success = "true",
                Pending = "false",
                ErrorOccured = "false",
                IsVoided = "false",
                IsRefunded = "false",
                Id = "gfhfgh123",
                Order = "ertret21",
                SourceDataPan = "1234567890123456",
                SourceDataType = "card",
                SourceDataSubType = "visa"
            };

            var order = new Order
            {
                UserId = 1,
                Status = OrderStatus.PendingPayment,
                Payment = new Payment
                {
                    Status = PaymentStatus.Pending
                }
            };


            var now = new DateTime(2026, 5, 8, 6, 56, 0, DateTimeKind.Utc);

            _paymentGatweyMock.Setup(x => x.IsCallbackAuthentic(callback)).Returns(true);
            _orderRepoMock.Setup(x => x.GetByMerchantOrderIdAsync("ECOM-123")).ReturnsAsync(order);
            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            // Act
            var actual = await _orderService.ProcessPaymentCallbackAsync(callback);

            // Assert
            actual.Should().NotBeNull();
            actual.IsSuccess.Should().BeTrue();

            order.Status.Should().Be(OrderStatus.Paid);
            order.Payment!.Status.Should().Be(PaymentStatus.Succeeded);

            _orderRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentCallbackAsync_ShouldReturnPending_WhenPaymentIsPending()
        {
            // Arrange
            var callback = new PaymentCallbackDto
            {
                MerchantOrderId = "ECOM-123",
                Success = "false",
                Pending = "true",
                ErrorOccured = "false",
                IsVoided = "false",
                IsRefunded = "false"
            };

            var order = new Order
            {
                UserId = 1,
                Status = OrderStatus.PendingPayment,
                Payment = new Payment
                {
                    Status = PaymentStatus.Pending
                }
            };

            _paymentGatweyMock.Setup(x => x.IsCallbackAuthentic(callback)).Returns(true);

            _orderRepoMock.Setup(x => x.GetByMerchantOrderIdAsync("ECOM-123")).ReturnsAsync(order);

            // Act
            var actual = await _orderService.ProcessPaymentCallbackAsync(callback);

            // Assert
            actual.Should().NotBeNull();
            actual.IsPending.Should().BeTrue();

            order.Status.Should().Be(OrderStatus.PendingPayment);
            order.Payment!.Status.Should().Be(PaymentStatus.Pending);

            _orderRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentCallbackAsync_ShouldReturnFailed_WhenPaymentIsDeclined()
        {
            // Arrange
            var callback = new PaymentCallbackDto
            {
                MerchantOrderId = "ECOM-123",
                Success = "false",
                Pending = "false",
                ErrorOccured = "true",
                IsVoided = "false",
                IsRefunded = "false"
            };

            var order = new Order
            {
                UserId = 1,
                Status = OrderStatus.PendingPayment,
                Payment = new Payment
                {
                    Status = PaymentStatus.Pending
                }
            };

            _paymentGatweyMock.Setup(x => x.IsCallbackAuthentic(callback)).Returns(true);

            _orderRepoMock.Setup(x => x.GetByMerchantOrderIdAsync("ECOM-123")).ReturnsAsync(order);

            // Act
            var result = await _orderService.ProcessPaymentCallbackAsync(callback);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.IsPending.Should().BeFalse();

            order.Status.Should().Be(OrderStatus.PaymentFailed);
            order.Payment!.Status.Should().Be(PaymentStatus.Failed);

            _orderRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
