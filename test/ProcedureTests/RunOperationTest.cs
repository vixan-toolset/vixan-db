using Vixan.Db.Test.Fixtures;

namespace Vixan.Db.Test.ProcedureTests;

[Collection("Sequential")]
public class RunOperationTest : BaseProcedureTest
{
    [Fact]
    public void RunOperation_Breaks_When_No_Process_Started()
    {
        // Arrange
        DbFixture.Reset();
        AssertNoCurrentProcess();
        var startTime = DbFixture.GetTimeUc();

        // Act
        DbFixture.RunOperation(1, 1);

        // Assert
        AssertSingleErrorSince(startTime, "dbo.RunOperation", "No process is currently started.");
        AssertNoCurrentOperation();
    }

    [Fact]
    public void RunOperation_Breaks_When_Worker_Undefined()
    {
        // Arrange
        DbFixture.Reset();
        AssertProcessStarted();
        var operationId = DbFixture.NextOperationToRun();
        var startTime = DbFixture.GetTimeUc();

        // Act
        DbFixture.RunOperation(operationId, 1);

        // Assert
        AssertSingleErrorSince(startTime, "dbo.RunOperation", "There is no current worker with the given identifier.");
        AssertNoOperationStarted();
    }

    [Fact]
    public void RunOperation_Breaks_When_Worker_Not_Started()
    {
        // Arrange
        DbFixture.Reset();
        AssertProcessStarted();
        AssertWorkerStartedThenStopped();
        var workerId = DbFixture.GetFirstCurrentWorker().WorkerId;
        var operationId = DbFixture.NextOperationToRun();
        var startTime = DbFixture.GetTimeUc();

        // Act
        DbFixture.RunOperation(operationId, (int)workerId);

        // Assert
        AssertSingleErrorSince(startTime, "dbo.RunOperation", "The worker with the given identifier is not started.");
        AssertNoOperationStarted();
    }

    [Fact]
    public void RunOperation_Breaks_When_Operation_Not_Planned()
    {
        // Arrange
        DbFixture.Reset();
        AssertProcessStarted();
        AssertWorkerStarted();
        var workerId = DbFixture.GetFirstCurrentWorker().WorkerId;
        var operationId = DbFixture.NextOperationToRun();
        DbFixture.TerminateOperation(operationId);
        var startTime = DbFixture.GetTimeUc();

        // Act
        DbFixture.RunOperation(operationId, (int)workerId);

        // Assert
        AssertSingleErrorSince(startTime, "dbo.RunOperation", "The operation is not planned to start.");
        AssertNoOperationStarted();
    }

    [Fact]
    public void RunOperation_Runs_Planned_Operation_With_Started_Worker()
    {
        // Arrange
        DbFixture.Reset();
        _ = AssertProcessStarted();
        AssertWorkerStarted();
        var workerId = DbFixture.GetFirstCurrentWorker().WorkerId;
        var operationId = DbFixture.NextOperationToRun();
        var startTime = DbFixture.GetTimeUc();

        // Act
        DbFixture.RunOperation(operationId, (int)workerId);

        // Assert
        AssertNoErrorSince(startTime);
        AssertStartedOperation(operationId);
    }
}