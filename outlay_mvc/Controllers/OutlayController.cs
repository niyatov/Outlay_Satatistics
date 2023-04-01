using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using outlay_mvc.Data;
using outlay_mvc.Dtoes;
using outlay_mvc.Entities;
using outlay_mvc.Filters;
using outlay_mvc.Models;
using System.Security.Claims;

namespace outlay_mvc.Controllers;
[Authorize]
public class OutlayController : Controller
{
    private readonly AppDbContext _context;

    public OutlayController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult CreateOutlay(string? error)
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var categories = _context.UserCategories.Where(x => x.UserId == userId).ToList();
        ViewBag.Categories = categories;

        if (error is not null)
        {
            ModelState.AddModelError("", error);
            return View(new CreateOutlayDto());
        }

        return View(new CreateOutlayDto());
    }

    [HttpPost]
    public async Task<IActionResult> CreateOutlay(CreateOutlayDto outlayVM)
    {
        if (!ModelState.IsValid)
        {
            List<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
            string errorsString = "";

            for (int i = 0; i < allErrors.Count(); i++)
            {
                errorsString += $"   {i + 1}) " + allErrors[i].ErrorMessage;
            }
            return Redirect($"CreateOutlay?error={errorsString}");
        }


        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var outlay = new Outlay()
        {
            Name = outlayVM.Name,
            Description = outlayVM.Description,
            Cost = outlayVM.Cost,
            UserId = userId,
            CategoryId = outlayVM.CategoryId,
        };

        await _context.Outlays.AddAsync(outlay);
        await _context.SaveChangesAsync();

        return Redirect($"GetOutlay?outlayId={outlay.Id}");
    }


    public IActionResult GetOutlay(int outlayId)
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var outlay = _context.Outlays.FirstOrDefault(x => x.Id == outlayId);

        if (outlay == null)
        {
            ModelState.AddModelError("", "outlay not found");
            return View();
        }

        var outlayVM = new GetOutlayVM()
        {
            Id = outlay.Id,
            CategoryId = outlay.CategoryId,
            Name = outlay.Name,
            Description = outlay.Description,
            CreateAt = outlay.CreateAt,
            LastUpdateAt = outlay.LastUpdateAt,
            Cost = outlay.Cost ?? 0,
            Username = outlay.User.UserName,
            IsAdmin = outlay.Category!.UserCategories!.Any(x => x.UserId == userId && x.IsAdmin == true),
            IsOwner = outlay.User.UserName == User.FindFirst(ClaimTypes.Name).Value
        };

        return View(outlayVM);
    }


    [OutlayFilter("Owner")]
    public IActionResult UpdateOutlay(int outlayId, string? error)
    {
        ViewData["outlayId"] = outlayId;

        if (error is not null)
        {
            ModelState.AddModelError("", error);
            return View();
        }

        return View();
    }

    [OutlayFilter("Owner")]
    [HttpPost]
    public async Task<IActionResult> UpdateOutlay(int outlayId, UpdateOutlayDto outlayVM)
    {
        if (!ModelState.IsValid)
        {
            List<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
            string errorsString = "";

            for (int i = 0; i < allErrors.Count(); i++)
            {
                errorsString += $"   {i + 1}) " + allErrors[i].ErrorMessage;
            }
            return Redirect($"UpdateOutlay?outlayId={outlayId}&&error={errorsString}");
        }

        var outlay = _context.Outlays.FirstOrDefault(x => x.Id == outlayId);

        outlay.Name = outlayVM.Name;
        outlay.Description = outlayVM.Description;
        outlay.Cost = outlayVM.Cost;

        _context.Outlays.Update(outlay);
        await _context.SaveChangesAsync();

        return LocalRedirect($"/Outlay/GetOutlay?outlayId={outlayId}");
    }


    [OutlayFilter("Owner")]
    public async Task<IActionResult> DeleteOutlay(int outlayId)
    {
        var outlay = _context.Outlays.FirstOrDefault(x => x.Id == outlayId);

        _context.Outlays.Remove(outlay);
        await _context.SaveChangesAsync();

        return LocalRedirect($"/Category/GetCategory?categoryId={outlay.CategoryId}");
    }
}
