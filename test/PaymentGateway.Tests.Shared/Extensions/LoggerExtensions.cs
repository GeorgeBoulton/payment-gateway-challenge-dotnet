using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace PaymentGateway.Tests.Shared.Extensions;

public static class LoggerExtensions
{
    public static void ReceivedLog<T>(this ILogger<T> logger, LogLevel level, string containsMessage)
    {
        logger.Received().Log(
            level,
            Arg.Any<EventId>(),
            Arg.Is<object>(state => state.ToString().Contains(containsMessage)),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception, string>>());
    }
}