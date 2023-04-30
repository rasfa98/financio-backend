using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace FinancioBackend.AuthorizationRequirements;

public static class BudgetRequirement
{
    public static OperationAuthorizationRequirement ReadRequirement =
        new OperationAuthorizationRequirement() { Name = nameof(ReadRequirement) };

    public static OperationAuthorizationRequirement UpdateRequirement =
        new OperationAuthorizationRequirement() { Name = nameof(UpdateRequirement) };
 
    public static OperationAuthorizationRequirement DeleteRequirement =
        new OperationAuthorizationRequirement() { Name = nameof(DeleteRequirement) };
}