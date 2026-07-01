using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOrganizer.Domain.Services
{
    public interface IEmailService
    {
        Task SendOTPAsync(string email, string userName, string otp);
    }
}
