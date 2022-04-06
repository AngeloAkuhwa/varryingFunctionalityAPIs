using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoAPI_app.DTOs
{
    public class UsermanagerResponseDTO
    {
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
