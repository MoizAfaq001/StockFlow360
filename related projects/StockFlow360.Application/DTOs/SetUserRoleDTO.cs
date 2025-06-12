using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow360.Application.DTOs
{
    public class SetUserRoleDTO
    {
        public string UserEmail { get; set; } = string.Empty;
        public string NewRole { get; set; } = string.Empty;
    }

}
