using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entities;

namespace LNU.Courses.WebUI.Models
{
    public class DisciplineViewModel
    {
        public DisciplineViewModel(Disciplines discipline)
        {
            id = discipline.id;
            name = discipline.name;
            kafedra = discipline.kafedra;
            lecturer = discipline.Lecturers;
            course = discipline.course;
            description = discipline.description;
        }

        public int id { get; set; }
        public string name { get; set; }
        public string kafedra { get; set; }
        public Lecturer lecturer { get; set; }
        public int course { get; set; }
        public int Semestr { get; set; }
        public string description { get; set; }
        public int firstWave { get; set; }
        public int secondWave { get; set; }
    }
}