using System;

namespace SampleSP.NET.Web.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId { get; set; }//=> !string.IsNullOrEmpty(RequestId);
    }
}
