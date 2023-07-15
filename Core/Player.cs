using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public partial class Player: BaseEntity
    {
        public string Email { get; set; }

        public decimal Balance { get; set; }

    }
}
