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
    
    public partial class LUONG
    {
        public int IDL { get; set; }
        public string MSNV { get; set; }
        public Nullable<int> LCB { get; set; }
        public Nullable<int> SONGAYLAM { get; set; }
        public Nullable<int> BHYT { get; set; }
        public Nullable<int> BHXH { get; set; }
        public Nullable<int> BHTN { get; set; }
        public Nullable<int> TONGPHUCAP { get; set; }
        public Nullable<int> SOGIOTANGCA { get; set; }
        public Nullable<int> TONGTIENTANGCA { get; set; }
        public Nullable<int> TONGTIENNHAN { get; set; }
        public Nullable<System.DateTime> THANG { get; set; }
    
        public virtual NHANVIEN NHANVIEN { get; set; }
    }
}
