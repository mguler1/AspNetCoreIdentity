using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspNetCoreIdentity.Extensions
{
    public static class ModelstateExtensions
    {
        public static void AddModelErrorList(this ModelStateDictionary modelState,List<string>errors )
        {
            errors.ForEach(x => 
            { 
              modelState.AddModelError(string.Empty, x);
            });
          
        }
    }
}
