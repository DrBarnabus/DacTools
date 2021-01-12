// Copyright (c) 2021 DrBarnabus

using DacTools.Deployment.Core.Logging;
using Moq;
using Xunit;

namespace DacTools.Deployment.Core.Tests.Logging
{
    public class LogExtensionsTests
    {
        [Fact]
        public void ShouldLogWithDebugLevel()
        {
            var mockLog = new Mock<ILog>();
            mockLog.Object.Debug("Message");
            mockLog.Verify(m => m.Write(LogLevel.Debug, "Message"), Times.Once);
        }

        [Fact]
        public void ShouldLogWithErrorLevel()
        {
            var mockLog = new Mock<ILog>();
            mockLog.Object.Error("Message");
            mockLog.Verify(m => m.Write(LogLevel.Error, "Message"), Times.Once);
        }

        [Fact]
        public void ShouldLogWithInfoLevel()
        {
            var mockLog = new Mock<ILog>();
            mockLog.Object.Info("Message");
            mockLog.Verify(m => m.Write(LogLevel.Info, "Message"), Times.Once);
        }

        [Fact]
        public void ShouldLogWithWarningLevel()
        {
            var mockLog = new Mock<ILog>();
            mockLog.Object.Warning("Message");
            mockLog.Verify(m => m.Write(LogLevel.Warn, "Message"), Times.Once);
        }
    }
}
