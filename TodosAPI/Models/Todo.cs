using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodosAPI.Models
{
    public class Todo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Title { get; set; }

        public string CreationDate { get; set; }

        public bool Done { get; set; }

        public string Priority { get; set; }
    }
}