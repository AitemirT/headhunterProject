using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace JobSearch.ViewModels;

public class EditUserViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Имя пользователя не может быть пустым")]
    [Remote(action: "CheckUserName", controller: "Validation", AdditionalFields = "UserName,Id", ErrorMessage = "Пользователь с таким именем пользователя уже существует")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Почта не может быть пустой")]
    [Remote(action: "CheckUserEmail", controller: "Validation", AdditionalFields = "Email,Id",ErrorMessage = "Пользователь с такой почтой уже существует")]
    [EmailAddress(ErrorMessage = "Почта в некорректном формате")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Номер телефона не может быть пустым")]
    public string PhoneNumber { get; set; }
    
    public IFormFile? Avatar { get; set; }
}