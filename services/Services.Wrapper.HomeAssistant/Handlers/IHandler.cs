using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Wrapper.HomeAssistant.Handlers
{
    public interface IHandler<T>
    {
        Task Execute(T model);
    }
}
