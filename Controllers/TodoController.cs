using System;
using System.Threading.Tasks;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreTodo.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoItemService _todoItemService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController(ITodoItemService todoItemService, UserManager<ApplicationUser> userManager)
        {
            _todoItemService = todoItemService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            // Get to-do items from database
            // Put items into a model
            // Pass the view to a model and render
            // return View();
            var currentUser = await _userManager.GetUserAsync(User);
            if(currentUser == null)
            {
                return Challenge();
            }
            var todoItems = await _todoItemService.GetIncompleteItemsAsync(currentUser);
            var model = new TodoViewModel()
            {
                Items = todoItems
            };

            return View(model);
        }
        public async Task<IActionResult> AddItem(NewTodoItem newItem)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null )
            {
                return Unauthorized();
            }

            var successful = await _todoItemService.AddItemAsync(newItem, currentUser);
            if (!successful)
            {
                return BadRequest(new { error = "Could not add item." });
            }

            return Ok();
        }
        public async Task<IActionResult> MarkDone(Guid id)
        {
            if (id == Guid.Empty) return BadRequest();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null )
            {
                return Unauthorized();
            }

            var successful = await _todoItemService.MarkDoneAsync(id, currentUser);
            if (!successful) return BadRequest();

            return Ok();
        }
    }
}