//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System.ComponentModel;
namespace Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Administrators
    {
        [DisplayName("Логін")]
        public string login { get; set; }
        [DisplayName("Пароль")]
        public string password { get; set; }
        public string roles { get; set; }
        public Nullable<int> lecturerId { get; set; }
    
        public virtual Lecturer Lecturers { get; set; }
    }
}
