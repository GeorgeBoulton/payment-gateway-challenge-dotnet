using AutoFixture;
using FluentAssertions;
using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.UnitTests.Mappers;

public class PaymentStatusMapperTests
{
    private readonly Fixture _fixture = new();

    private readonly PaymentStatusMapper _sut = new PaymentStatusMapper();
    
    private static IEnumerable<TestCaseData> StatusCases =>
        new[]
        {
            new TestCaseData(
                Domain.Entities.PaymentStatus.Authorized,
                PaymentStatus.Authorized),
        
            new TestCaseData(
                Domain.Entities.PaymentStatus.Declined,
                PaymentStatus.Declined),
        
            new TestCaseData(
                Domain.Entities.PaymentStatus.Rejected,
                PaymentStatus.Rejected)
        };

    [TestCaseSource(nameof(StatusCases))]
    public void Map_GivenDomainPaymentStatus_ReturnsPaymentStatus(
        Domain.Entities.PaymentStatus domainStatus,
        PaymentStatus expectedStatus)
    {
        //Act
        var result = _sut.Map(domainStatus);
        
        // Assert
        result.Should().Be(expectedStatus);
    }
}