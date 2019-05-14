using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace LinkAgregator.Data.Models
{
    public class Link
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int Rate { get; set; }
        public Guid userId { get; set; }
    }
}
