using System.Security.Claims;
using FinancioBackend.AuthorizationRequirements;
using FinancioBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace FinancioBackend.AuthorizationHandlers;

public class BudgetAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Budget>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Budget budget)
    {
        var userClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        int userId = int.TryParse(userClaim, out var id) ? id : 0;

        switch (requirement.Name)
        {
            case nameof(BudgetRequirement.ReadRequirement):
            case nameof(BudgetRequirement.UpdateRequirement):
            case nameof(BudgetRequirement.DeleteRequirement):
                {
                    if (userId == budget.UserId)
                    {
                        context.Succeed(requirement);
                    }

                    break;
                }
        }

        return Task.CompletedTask;
    }
}