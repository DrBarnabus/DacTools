// Copyright (c) 2022 DrBarnabus

using System;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.AsyncTasks;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.DatabaseListGenerators;
using DacTools.Deployment.Core.Logging;
using DacTools.Deployment.Core.Tests.TestInfrastructure;
using Moq;
using Shouldly;
using Xunit;

namespace DacTools.Deployment.Core.Tests.AsyncTasks;

public class AsyncTaskBaseTests
{
    [Fact]
    public async Task ShouldCallTheCorrectLoggingMethods()
    {
        // Setup
        var arguments = new Arguments();
        var mockLog = new Mock<ILog>();
        var mockBuildServer = new Mock<IActiveBuildServer>();
        var sut = new TestAsyncTask2(arguments, mockLog.Object, mockBuildServer.Object);
        sut.Setup(new DatabaseInfo(1, "Test"), (_, _, _) => { });

        // Act
        await sut.Run(CancellationToken.None);

        // Assert
        mockLog.Verify(m => m.Write(LogLevel.Debug, @"'Test:1' Internal - Test"), Times.Once);
        mockLog.Verify(m => m.Write(LogLevel.Info, "'Test:1' Internal - Test"), Times.Once);
        mockLog.Verify(m => m.Write(LogLevel.Warn, "'Test:1' Internal - Test"), Times.Once);
        mockLog.Verify(m => m.Write(LogLevel.Error, "'Test:1' Internal - Test"), Times.Once);
    }

    [Fact]
    public async Task ShouldSetTheProgressUpdateDelegateCorrectly()
    {
        // Setup
        bool delegateCalled = false;
        var databaseInfo = new DatabaseInfo(1, "Test");

        var mockLog = new Mock<ILog>();
        var arguments = new Arguments();
        var mockBuildServer = new Mock<IActiveBuildServer>();
        var sut = new TestAsyncTask2(arguments, mockLog.Object, mockBuildServer.Object);

        void ProgressUpdate(IAsyncTask asyncTask, bool succeeded, long elapsedMiliseconds)
        {
            delegateCalled = true;
            asyncTask.DatabaseInfo.ShouldBe(databaseInfo);
            succeeded.ShouldBeTrue();
            elapsedMiliseconds.ShouldBe(100);
        }

        sut.Setup(databaseInfo, ProgressUpdate);

        // Act
        await sut.Run(CancellationToken.None);

        // Assert
        delegateCalled.ShouldBeTrue();
        sut.PublicProgressUpdate.ShouldBe(ProgressUpdate);
    }

    [Fact]
    public void ShouldThrowAnArgumentNullExceptionWhenDatabaseInfoIsNull()
    {
        // Setup
        var mockLog = new Mock<ILog>();
        var arguments = new Arguments();
        var mockBuildServer = new Mock<IActiveBuildServer>();
        var sut = new TestAsyncTask2(arguments, mockLog.Object, mockBuildServer.Object);

        // Assert
        Assert.Throws<ArgumentNullException>(() => sut.Setup(null!, (_, _, _) => { }));
    }

    [Fact]
    public void ShouldThrowAnArgumentNullExceptionWhenProgressUpdateDelegateIsNull()
    {
        // Setup
        var mockLog = new Mock<ILog>();
        var arguments = new Arguments();
        var mockBuildServer = new Mock<IActiveBuildServer>();
        var sut = new TestAsyncTask2(arguments, mockLog.Object, mockBuildServer.Object);

        // Assert
        Assert.Throws<ArgumentNullException>(() => sut.Setup(new DatabaseInfo(1, "Test"), null!));
    }
}
