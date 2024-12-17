using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace JobSearch.ViewModels;

public class RegisterViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Логин не может быть пустым")]
    [Remote(action: "CheckUserName", controller: "Validation", ErrorMessage = "Пользователь с таким именем пользователя уже существует")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Почта не может быть пустой")]
    [EmailAddress(ErrorMessage = "Почта в некорректном формате")]
    [Remote(action: "CheckUserEmail", controller: "Validation", ErrorMessage = "Пользователь с такой почтой уже существует", AdditionalFields = "Id")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Номер телефона не может быть пустым")]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; }
    public IFormFile? Avatar { get; set; }
    [Required(ErrorMessage = "Пароль не может быть пустым")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "Повтор пароля не может быть пустым")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    
    [Required(ErrorMessage = "Выберите роль")]
    public string Role { get; set; }
}