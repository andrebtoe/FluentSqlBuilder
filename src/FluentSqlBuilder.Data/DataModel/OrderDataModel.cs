﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FluentSqlBuilder.Data.DataModel
{
    [Table("Order", Schema = "Checkout")]
    public class OrderDataModel
    {
        public int Id { get; set; }
        [Column("customer_id")]
        public int CustomerId { get; set; }
        public DateTime DateTime { get; set; }
        public OrderStatus Status { get; set; }
    }
}