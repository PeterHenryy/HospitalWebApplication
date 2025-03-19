﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HospitalWebApplication.Models
{
    public class BillItem
    {
        [Key]
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        // References
        [ForeignKey("Bill")]
        public int BillId { get; set; }
        public virtual Bill Bill { get; set; }
    }
}
