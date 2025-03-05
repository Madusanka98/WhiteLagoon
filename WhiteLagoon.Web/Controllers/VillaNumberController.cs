using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var villaNumbers = _unitOfWork.villaNumber.GetAll(includeProperties:"Villa"); 
            return View(villaNumbers);
        }

        public IActionResult Create()
        {
            VillaNumberVM villaNumberVM = new()
            {
                villaList = _unitOfWork.villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM obj)
        {
            bool roomNumberExists = _unitOfWork.villaNumber.Any(u => u.Villa_Number == obj.villaNumber.Villa_Number);
            if (ModelState.IsValid && !roomNumberExists)
            {
                _unitOfWork.villaNumber.Add(obj.villaNumber);
                _unitOfWork.Save();
                TempData["success"] = "The villa number has been Created successfully.";
                return RedirectToAction(nameof(Index));
            }
            if (roomNumberExists)
            {
                TempData["error"] = "The villa number already exists.";
            }
            obj.villaList = _unitOfWork.villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }

        public IActionResult Update(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                villaList = _unitOfWork.villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                villaNumber = _unitOfWork.villaNumber.Get(u=>u.Villa_Number== villaNumberId)
            };

            if(villaNumberVM.villaNumber  == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {
            bool roomNumberExists = _unitOfWork.villaNumber.Any(u => u.Villa_Number == villaNumberVM.villaNumber.Villa_Number);
            if (ModelState.IsValid)
            {
                _unitOfWork.villaNumber.Update(villaNumberVM.villaNumber);
                _unitOfWork.Save();
                TempData["success"] = "The villa number has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            villaNumberVM.villaList = _unitOfWork.villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(villaNumberVM);

        }

        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM villaNumberVM = new()
            {
                villaList = _unitOfWork.villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                villaNumber = _unitOfWork.villaNumber.Get(u => u.Villa_Number == villaNumberId)
            };

            if (villaNumberVM.villaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaNumberVM);
        }

        [HttpPost]
        public IActionResult Delete(VillaNumberVM VillaNumberVM)
        {
            VillaNumber? objFromDb = _unitOfWork.villaNumber.Get(x => x.Villa_Number == VillaNumberVM.villaNumber.Villa_Number);

            if (objFromDb is not null)
            {
                _unitOfWork.villaNumber.Remove(objFromDb);
                _unitOfWork.Save();
                TempData["success"] = "The villa number has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["success"] = "The villa number could not be deleted.";
            return View();
        }
    }
}
