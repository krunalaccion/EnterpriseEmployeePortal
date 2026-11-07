using EmployeePortal.Business.Services;
using EmployeePortal.Business.Validation;
using EmployeePortal.Data.Repositories;
using EmployeePortal.Models.DTOs;
using EmployeePortal.Models.Enums;
using EmployeePortal.UnitTests.Fixtures;
using Xunit;

namespace EmployeePortal.UnitTests;

public class LeaveAndAttendanceServiceTests
{
    [Fact]
    public async Task SubmitAndReviewLeaveRequestAsync_UpdatesApprovalStatus()
    {
        await using var db = await InMemoryPortalTestFixture.CreateSeededDbContextAsync();
        var service = new AttendanceAndLeaveService(
            new AttendanceRepository(db),
            new LeaveRepository(db),
            new EmployeeRepository(db),
            new InputValidationService());

        var submitted = await service.SubmitLeaveRequestAsync(new SubmitLeaveRequestDto
        {
            EmployeeId = 2,
            LeaveType = LeaveType.AnnualVacation,
            StartDate = DateTime.UtcNow.Date.AddDays(5),
            EndDate = DateTime.UtcNow.Date.AddDays(7),
            Reason = "Unit test vacation"
        });

        Assert.Equal(3m, submitted.TotalDays);
        Assert.Equal(LeaveApprovalStatus.Pending, submitted.Status);

        var reviewed = await service.ReviewLeaveRequestAsync(new ReviewLeaveRequestDto
        {
            LeaveRequestId = submitted.Id,
            NewStatus = LeaveApprovalStatus.Approved,
            ReviewerComments = "Approved in test"
        }, reviewerUserId: 1);

        Assert.NotNull(reviewed);
        Assert.Equal(LeaveApprovalStatus.Approved, reviewed!.Status);
    }

    [Fact]
    public async Task SubmitLeaveRequestAsync_WithInvalidDateOrder_ThrowsArgumentException()
    {
        await using var db = await InMemoryPortalTestFixture.CreateSeededDbContextAsync();
        var service = new AttendanceAndLeaveService(
            new AttendanceRepository(db),
            new LeaveRepository(db),
            new EmployeeRepository(db),
            new InputValidationService());

        await Assert.ThrowsAsync<ArgumentException>(() => service.SubmitLeaveRequestAsync(new SubmitLeaveRequestDto
        {
            EmployeeId = 1,
            StartDate = DateTime.UtcNow.Date.AddDays(10),
            EndDate = DateTime.UtcNow.Date.AddDays(5),
            Reason = "Invalid range"
        }));
    }
}

