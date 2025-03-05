using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.ViewModels
{
    public class VillaNumberVM
    {
        public VillaNumber? villaNumber { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? villaList { get; set; }
    }
}
