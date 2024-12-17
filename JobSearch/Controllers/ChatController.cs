using JobSearch.Models;
using JobSearch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly JobSearchDbContext _context;
    private readonly UserManager<MyUser> _userManager;

    public ChatController(JobSearchDbContext context, UserManager<MyUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int chatId)
    {
        Chat? chat = await _context.Chats
            .Include(c => c.Messages)
            .ThenInclude(m => m.Sender)
            .Include(c => c.Respond)
            .ThenInclude(r => r.Vacancy)
            .Include(c => c.Respond)
            .ThenInclude(r => r.Cv)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
        {
            return NotFound();
        }
        return View(chat);
    }
    
    [HttpPost]
    public async Task<IActionResult> SendMessage(int chatId, string messageText)
    {
        if (string.IsNullOrEmpty(messageText))
        {
            return BadRequest("Сообщение не может быть пустым.");
        }
        
        Chat? chat = await _context.Chats
            .Include(c => c.JobSeeker)
            .Include(c => c.Employer)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
        {
            return NotFound("Чат не найден.");
        }


        MyUser sender = await _userManager.GetUserAsync(User);

        if (sender == null)
        {
            return Unauthorized("Пользователь не найден.");
        }
        
        if (chat.JobSeekerID != sender.Id && chat.EmployerID != sender.Id)
        {
            return Unauthorized("Вы не можете отправить сообщение в этот чат.");
        }
        
        Message newMessage = new Message
        {
            Text = messageText,
            DateOfSend = DateTime.UtcNow, SenderId = sender.Id,
            ChatId = chat.Id
        };
        _context.Messages.Add(newMessage);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", new { chatId = chat.Id });
    }
}