using AutoFixture;
using FluentAssertions;
using NSubstitute;
using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Domain.Entities;

using PaymentStatus = PaymentGateway.Api.Models.PaymentStatus;

namespace PaymentGateway.Api.UnitTests.Mappers;

[TestFixture]
public class GetPaymentResponseMapperTests
{
    private readonly Fixture _fixture = new();
    
    private readonly IPaymentStatusMapper _paymentStatusMapper = Substitute.For<IPaymentStatusMapper>();

    private GetPaymentResponseMapper _sut;
    
    [SetUp]
    public void SetUp()
    {
        _sut = new GetPaymentResponseMapper(_paymentStatusMapper);
    }
    
    [Test]
    public void Map_GivenPayment_ReturnsGetPaymentResponseWithCorrectFields()
    {
        // Arrange
        var expectedStatus = _fixture.Create<PaymentStatus>();
        var payment = _fixture.Create<Payment>();
        
        _paymentStatusMapper
            .Map(payment.Status)
            .Returns(expectedStatus);

        // Act
        var result = _sut.Map(payment);

        // Assert
        _paymentStatusMapper.Received(1).Map(payment.Status);

        var expected = new GetPaymentResponse(
            payment.Id,
            expectedStatus,
            payment.CardNumber,
            payment.ExpiryMonth,
            payment.ExpiryYear,
            payment.Currency,
            payment.Amount
        );

        result.Should().BeEquivalentTo(expected);
    }
}