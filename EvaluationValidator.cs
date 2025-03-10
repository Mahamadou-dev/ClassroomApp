using System.ComponentModel.DataAnnotations;

namespace Backend
{
    public static class EvaluationValidator
    {
        public static ValidationResult ValiderDateLimite(DateTimeOffset date, ValidationContext context)
        {
            return date >= DateTimeOffset.UtcNow ? ValidationResult.Success!
                  : new ValidationResult("La date limite doit être dans le futur.");
        }
    }

}
