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
    
    public partial class Students
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Students()
        {
            this.StudentsInGroups = new HashSet<StudentsInGroups>();
        }
        [DisplayName("����� ��������")]
        public string id { get; set; }
        [DisplayName("�.�.�")]
        public string fio { get; set; }
        [DisplayName("����")]
        public int course { get; set; }
        [DisplayName("�����")]
        public string group { get; set; }
        [DisplayName("������� ������")]
        public double AverageMark { get; set; }
        [DisplayName("�����")]
        public string eMail { get; set; }
        [DisplayName("�������")]
        public string phoneNumber { get; set; }
        public bool Deleted { get; set; }
        public bool locked { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentsInGroups> StudentsInGroups { get; set; }
        public virtual Users Users { get; set; }
    }
}
