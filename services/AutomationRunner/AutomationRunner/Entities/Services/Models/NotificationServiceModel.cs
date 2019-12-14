using System;
using System.Collections.Generic;
using System.Text;

namespace AutomationRunner.Entities.Services.Models
{
    public class NotificationServiceModel : IService
    {
        public string Message { get; set; }

        public NotificationServiceModel(string message)
        {
            Message = message;
        }
    }
}
