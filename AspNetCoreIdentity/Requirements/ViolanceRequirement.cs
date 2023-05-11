using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreIdentity.Requirements
{
	public class ViolanceRequirement : IAuthorizationRequirement
	{
		public int ThresholdAge { get; set; }
	}

	public class ViolanceRequirementHandler : AuthorizationHandler<ViolanceRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolanceRequirement requirement)
		{
			if (!context.User.HasClaim(x => x.Type == "birthdate"))
			{
				context.Fail();
				return Task.CompletedTask;
			}

			Claim birthDateClaim = context.User.FindFirst("birthdate")!;


			var today = DateTime.Now;
			var birthDate = Convert.ToDateTime(birthDateClaim.Value);
			var age = today.Year - birthDate.Year;


			if (birthDate > today.AddYears(-age)) age--; // artık yıl hesabı



			if (requirement.ThresholdAge > age)
			{
				context.Fail();
				return Task.CompletedTask;
			}

			context.Succeed(requirement);
			return Task.CompletedTask;
		}
	}
}