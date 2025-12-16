using FluentAssertions;

using PaymentGateway.DAL.Entities;
using PaymentGateway.DAL.Mappers;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.DAL.UnitTests.Mappers;

public class StatusMapperTests
{
    private readonly StatusMapper _sut = new StatusMapper();

    private static IEnumerable<TestCaseData> StatusCases =>
        new[]
        {
            new TestCaseData(
                PaymentStatus.Authorized,
                Status.Authorized),
            new TestCaseData(
                PaymentStatus.Declined,
                Status.Declined),
        };
    
    [TestCaseSource(nameof(StatusCases))]    
    public void Map_GivenPaymentStatus_ReturnsStatus(PaymentStatus paymentStatus, Status status)
    {
        // Act
        var result = _sut.Map(paymentStatus);

        // Assert
        result.Should().Be(status);
    }
    
    [TestCaseSource(nameof(StatusCases))]
    public void Map_GivenStatus_ReturnsPaymentStatus(PaymentStatus paymentStatus, Status status)
    {
        // Act
        var result = _sut.Map(status);

        // Assert
        result.Should().Be(paymentStatus);
    }

    [Test]
    public void Map_GivenRejectedPaymentStatus_ThrowsException()
    {
        // Act & Assert
        FluentActions
            .Invoking(() => _sut.Map(PaymentStatus.Rejected))
            .Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("*not valid in DAL*"); // matches the message in StatusMapper
    }
}