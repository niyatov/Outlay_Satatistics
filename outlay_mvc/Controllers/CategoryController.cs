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
public class CategoryController : Controller
{
    private readonly AppDbContext _context;
    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult CreateCategory() => View();

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto categoryVM)
    {
        if (!ModelState.IsValid) return View();
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        if (_context.Categories.Any(x => x.Name == categoryVM.Name))
        {
            ModelState.AddModelError("Name", "Name exists");
            return View();
        }

        var category = new Category()
        {
            Name = categoryVM.Name,
            Description = categoryVM.Description,
            Key = categoryVM.Key,
            IsPrivate = categoryVM.Key == null,
            UserCategories = new List<UserCategory>()
            {
                new UserCategory()
                {
                    UserId =  userId,
                    IsAdmin = true
                }
            }
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return RedirectToAction("GetCategories");
    }

    public IActionResult GetCategories(int size, int page)
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var userCategories = _context.UserCategories.Where(x => x.UserId == userId).ToList();

        List<GetCategoriesVM> categories = new List<GetCategoriesVM>();

        foreach (var usercategory in userCategories)
        {
            var category = new GetCategoriesVM()
            {
                Id = usercategory.Category.Id,
                Name = usercategory.Category.Name,
                Description = usercategory.Category.Description,
                NumberOfOutlays = usercategory.Category.Outlays == null ? 0 : usercategory.Category.Outlays.Count(),
                IsAdmin = usercategory.IsAdmin,
                Type = usercategory.Category.IsPrivate == true ? "Private" : "Public"
            };
            categories.Add(category);
        }

        var categoriesFilter = categories.ToPagedList<GetCategoriesVM>(new PaginationParams()
        {
            Page = page,
            Size = size
        });

        var result = new FilterGetCategories()
        {
            PaginationParams = new PaginationParams()
            {
                Size = size,
                Page = page,
                IsBack = categoriesFilter.Item2,
                IsNext = categoriesFilter.Item3
            },
            GetCategories = categoriesFilter.Item1 == null ? null : categoriesFilter.Item1.ToList()
        };

        return View(result);
    }


    public IActionResult GetCategory(int categoryId, int size, int page)
    {

        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var userCategory = _context.UserCategories.FirstOrDefault(x => x.UserId == userId && x.CategoryId == categoryId);

        if (userCategory is null)
        {
            ModelState.AddModelError("", "category not found");
            return View();
        }

        List<GetOutlaysVM>? outlays = new List<GetOutlaysVM>();

        if ((userCategory.Category.Outlays?.Count() > 0) == true)
        {
            foreach (var outlay in userCategory.Category.Outlays)
            {
                var outlayVM = new GetOutlaysVM()
                {
                    Id = outlay.Id,
                    Name = outlay.Name,
                    Description = outlay.Description,
                    Cost = outlay.Cost ?? 0,
                    Username = outlay.User.UserName
                };
                outlays.Add(outlayVM);
            }
        }

        var outlaysFilter = outlays.ToPagedList<GetOutlaysVM>(new PaginationParams()
        {
            Page = page,
            Size = size
        });

        GetCategoryVM category = new GetCategoryVM();

        category.Id = categoryId;
        category.Name = userCategory?.Category.Name;
        category.Description = userCategory.Category.Description;
        category.CreateAt = userCategory.Category.CreateAt;
        category.LastUpdateAt = userCategory.Category.LastUpdateAt;
        category.IsAdmin = userCategory.IsAdmin;
        category.Type = userCategory.Category.IsPrivate == true ? "Private" : "Public";
        category.Key = userCategory.Category.Key;
        category.FilterGetOutlays = new FilterGetOutlays()
        {
            PaginationParams = new PaginationParams()
            {
                Size = size,
                Page = page,
                IsBack = outlaysFilter.Item2,
                IsNext = outlaysFilter.Item3
            },
            Outlays = outlaysFilter.Item1 == null ? null : outlaysFilter.Item1.ToList()
        };

        return View(category);
    }

    [CategoryFilter("Admin")]
    [HttpGet]
    public IActionResult UpdateCategory(int categoryId, string? error)
    {
        var category = _context.Categories.First(x => x.Id == categoryId);

        ViewData["IsPrivate"] = category!.IsPrivate == true ? 1 : 0;
        ViewData["categoryId"] = categoryId;
        
        if (error is not null)
        {
            ModelState.AddModelError("", error);
            return View();
        }

        return View();
    }

    [CategoryFilter("Admin")]
    [HttpPost]
    public async Task<IActionResult> UpdateCategory(int categoryId,int IsPrivate, UpdateCategoryDto categoryVM)
    {
        if (!ModelState.IsValid || (IsPrivate == 0 && categoryVM.Key == null))
        {
            List<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
            string errorsString = "";

            for (int i = 0; i < allErrors.Count(); i++)
            {
                errorsString += $"   {i + 1}) " + allErrors[i].ErrorMessage;
            }

            if(IsPrivate == 0)
            {
                errorsString += $"   {allErrors.Count() + 1}) " + "The Key field is required.";
            }

            return Redirect($"UpdateCategory?categoryId={categoryId}&&error={errorsString}");
        }

        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var category = _context.Categories.FirstOrDefault(x => x.Id == categoryId);

        if (category is null)
        {
            ModelState.AddModelError("", "category not found");
            return View();
        }

        if (category.IsPrivate == false)
        {
            category.Key = categoryVM.Key;
        }

        category.Name = categoryVM.Name;
        category.Description = categoryVM.Description;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return LocalRedirect($"/Category/GetCategory?categoryId={categoryId}");
    }


    [CategoryFilter("Admin")]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        var category = _context.Categories.First(x => x.Id == categoryId);
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return RedirectToAction("GetCategories");
    }


    public IActionResult JoinCategory() => View();

    [HttpPost]
    public async Task<IActionResult> JoinCategory(JoinCategoryDto categoryVM)
    {
        if (!ModelState.IsValid) return View();
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var category = _context.Categories.FirstOrDefault(x => x.Name == categoryVM.Name && x.Key == categoryVM.Key);

        if (category is null)
        {
            ModelState.AddModelError("", "Name or Key incorrect");
            return View();
        }
        if (category.UserCategories.Any(x => x.UserId == userId))
        {
            ModelState.AddModelError("", "You already are in this category");
            return View();
        }
        var userCategory = new UserCategory()
        {
            UserId = userId,
            CategoryId = category.Id,
        };

        await _context.UserCategories.AddAsync(userCategory);
        await _context.SaveChangesAsync();

        return LocalRedirect($"/Category/GetCategory?categoryId={category.Id}");
    }


    public IActionResult ShowStatistics(int categoryId)
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var category = _context.Categories.FirstOrDefault(x => x.Id == categoryId);

        if (category is null)
        {
            ModelState.AddModelError("", "category not found");
            return View();
        }

        var averageSum = (category.Outlays.Select(x => x.Cost).Sum() ?? 0) / category.UserCategories.Count();

        var statistics = new GetStatisticsVM();
        var dict = new Dictionary<string, int>();
        string username;
        int money;

        foreach (var user in category.UserCategories)
        {
            var userMoney = category.Outlays.Where(x => x.UserId == user.UserId).Select(x => x.Cost).Sum() ?? 0;

            username = user.User.UserName;
            money = userMoney - averageSum;
            dict.Add(username, money);
        }
        statistics.Dict = dict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        statistics.CategoryId = categoryId;
        statistics.IsPrivate = category.IsPrivate;
        statistics.NumberOfPeople = category.UserCategories.Count();
        statistics.NumberOfOutlays = category.Outlays.Count();
        statistics.NumberOfYourOutlays = category.Outlays.Where(x => x.UserId == userId).Count();
        statistics.TotalSum = category.Outlays.Select(x => x.Cost).Sum() ?? 0;
        statistics.StartedAt = category.Outlays.OrderBy(x => x.CreateAt).First().CreateAt;
        statistics.FinishedAt = category.Outlays.OrderByDescending(x => x.CreateAt).First().CreateAt;
        statistics.SpentMoney = category.Outlays.Where(x => x.UserId == userId).Select(x => x.Cost).Sum() ?? 0;
        statistics.ResultMoney = statistics.Dict[User.FindFirst(ClaimTypes.Name).Value];
        statistics.IsAdmin = category.UserCategories.First(x => x.UserId == userId).IsAdmin;
        return View(statistics);
    }


    [CategoryFilter("Admin")]
    public async Task<IActionResult> RefreshCategory(int categoryId)
    {
        var outlays = _context.Outlays.Where(x => x.CategoryId == categoryId);
        if (outlays is null)
        {
            ModelState.AddModelError("", "outlay not found");
            return View();
        }

        _context.Outlays.RemoveRange(outlays);
        await _context.SaveChangesAsync();

        return LocalRedirect($"/Category/GetCategory?categoryId={categoryId}");
    }


    public IActionResult ShowUsers(int categoryId)
    {
        var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var users = _context.UserCategories.Where(x => x.CategoryId == categoryId).Select(x => x.User).ToList();
        var outlays = _context.Outlays.Where(x => x.CategoryId == categoryId);
        if (users is null)
        {
            ModelState.AddModelError("", "users not found");
            return View();
        }

        GetShowUsersVM usersVM = new GetShowUsersVM();
        usersVM.UsersVM = new List<GetShowUserVM>();


        var averageSum = (outlays.Select(x => x.Cost).Sum() ?? 0) / (users.Count() == 0 ? 1 : users.Count());
        int userMoney;

        foreach (var user in users)
        {
            GetShowUserVM userVM = new GetShowUserVM();
            userVM.UserName = user.UserName;


            userMoney = outlays.Where(x => x.UserId == user.Id).Select(x => x.Cost).Sum() ?? 0;

            userVM.ResultMoney = userMoney - averageSum;
            userVM.NumbersOfBuyingThings = outlays.Where(x => x.UserId == user.Id).Count();
            userVM.Id = user.Id;
            usersVM.UsersVM.Add(userVM);
        }
        usersVM.UsersVM = usersVM.UsersVM.OrderByDescending(x => x.ResultMoney).ToList();
        usersVM.CategoryId = categoryId;
        usersVM.IsAdmin = _context.UserCategories.First(x => x.CategoryId == categoryId && x.UserId == userId).IsAdmin;

        return View(usersVM);
    }

    [CategoryFilter("Admin")]
    public async Task<IActionResult> DeleteUser(int categoryId, int userId)
    {
        var userid = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var userCategory = _context.UserCategories.FirstOrDefault(x => x.UserId == userId && x.CategoryId == categoryId);

        var outlays = _context.Outlays.FirstOrDefault(x => x.CategoryId == categoryId && x.UserId == userId);
        if (outlays != null)
            _context.Outlays.Remove(outlays);

        _context.UserCategories.Remove(userCategory);
        await _context.SaveChangesAsync();

        if (userid == userId)
        {
            var category = _context.Categories.First(x => x.Id == categoryId);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction($"GetCategories");
        }

        return LocalRedirect($"/Category/ShowUsers?categoryId={categoryId}");
    }

}
