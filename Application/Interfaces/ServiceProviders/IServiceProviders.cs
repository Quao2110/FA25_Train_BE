using Application.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceProviders
{
    public interface IServiceProviders 
    {
        IUserService UserService { get; }
    }
}
