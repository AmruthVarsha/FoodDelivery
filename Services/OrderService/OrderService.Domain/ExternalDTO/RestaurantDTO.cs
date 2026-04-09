using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService.Domain.ExternalDTO
{
    public class RestaurantDTO
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
    }
}
