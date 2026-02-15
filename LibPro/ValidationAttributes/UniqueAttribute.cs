using LibPro.Models;
using System.ComponentModel.DataAnnotations;

namespace LibPro.ValidationAttributes
{
    public class UniqueAttribute
    {
        public class PubNameCheck : ValidationAttribute
        {

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                LibproContext _context = (LibproContext)validationContext.GetService(typeof(LibproContext))!;

                // 取得目前驗證的物件
                var instance = validationContext.ObjectInstance as Publishers;
                long? currentId = null;
                if (instance != null)
                {
                    currentId = instance.PubID;
                }

                // 查詢名稱重複，並排除自己
                var rulst = _context.Publishers
                    .Where(c => c.PubName == value.ToString() && c.PubID != currentId);

                if (rulst.Any())
                {
                    return new ValidationResult("出版商名稱重複");
                }
                return ValidationResult.Success;
            }
        }
    }
}
