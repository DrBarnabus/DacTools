// Copyright (c) 2022 DrBarnabus

using System;
using DacTools.Deployment.Core.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace DacTools.Deployment.Core.Tests.Logging
{
    public class LogTests
    {
        [Fact]
        public void ShouldAddANewLogAppender()
        {
            var consoleAppender = new ConsoleAppender();
            var log = new Log();
            log.AddLogAppender(consoleAppender);
            log.LogAppenders.ShouldBe(new[] { consoleAppender });
        }

        [Fact]
        public void ShouldCallWriteToOnAllLogAppenders()
        {
            var mockLogAppender = new Mock<ILogAppender>();
            var log = new Log(mockLogAppender.Object);
            log.Write(LogLevel.Info, "Message");
            mockLogAppender.Verify(m => m.WriteTo(LogLevel.Info, It.Is<string>(s => s.Length == 36)), Times.Once);
        }

        [Fact]
        public void ShouldConstructWithAnEmptyArrayOfLogAppenders()
        {
            var log = new Log();
            log.LogAppenders.ShouldBe(Array.Empty<ILogAppender>());
            log.LogLevel.ShouldBe(LogLevel.Info);
        }

        [Fact]
        public void ShouldConstructWithTheSuppliedArrayOfLogAppenders()
        {
            var consoleAppender = new ConsoleAppender();
            var log = new Log(consoleAppender);
            log.LogAppenders.ShouldBe(new[] { consoleAppender });
            log.LogLevel.ShouldBe(LogLevel.Info);
        }

        [Fact]
        public void ShouldReturnStringBuilderContentsWhenToStringIsCalled()
        {
            var log = new Log();
            log.Write(LogLevel.Info, "Message");

            string result = log.ToString();
            result.ShouldNotBeNull();
            result.Length.ShouldBe(36); // Expected Length of 1 Log Message
        }

        [Fact]
        public void ShouldReturnWithoutWritingWhenLogLevelIsNotActive()
        {
            var log = new Log { LogLevel = LogLevel.Warn };
            log.Write(LogLevel.Info, "Message");

            string result = log.ToString();
            result.ShouldNotBeNull();
            result.Length.ShouldBe(0);
        }
    }
}
