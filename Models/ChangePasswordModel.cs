using System.ComponentModel.DataAnnotations;

public class ChangePasswordModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Старый пароль")]
    public string OldPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Новый пароль")]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
    [Display(Name = "Подтверждение нового пароля")]
    public string ConfirmPassword { get; set; }
}