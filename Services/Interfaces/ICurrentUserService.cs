using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace Services.Interfaces
{
    public interface ICurrentUserService
    {
        CurrentUserInfo GetCurrentUser();
    }

    public class CurrentUserInfo
    {
        public int AccountId { get; set; }

        public string Username { get; set; } = null!;

        public AccountRole Role { get; set; }

        public int? KitchenId { get; set; }
    }
}
