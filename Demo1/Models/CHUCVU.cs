//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Demo1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CHUCVU
    {
        public int IDCHUCVU { get; set; }
        public string MSNV { get; set; }
        public string VITRI { get; set; }
        public string CHUCVU1 { get; set; }
    
        public virtual NHANVIEN NHANVIEN { get; set; }
    }
}
