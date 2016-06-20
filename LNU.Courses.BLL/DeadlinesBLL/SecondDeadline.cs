using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using LNU.Courses.Repositories;

namespace LNU.Courses.BLL.DeadlinesBLL
{
    public class SecondDeadline
    {
        private readonly IRepository _repository;
        private const int MaxCourse = 5;

        public SecondDeadline(IRepository repository)
        {
            _repository = repository;
        }
        private void manUpGroupsFirstly(int course)
        {
            var students = _repository.GetStudents().Where(std => std.course == course);
            var groups = _repository.GetGroups().Where(gr =>
            {
                var singleOrDefault = _repository.GetDisciplines().SingleOrDefault(d => d.id == gr.disciplinesID);
                return singleOrDefault != null && singleOrDefault.course == course && gr.Deleted == false && gr.Wave == 2;
            });

            var groupsFrstWave = _repository.GetGroups().Where(gr =>
            {
                var singleOrDefault = _repository.GetDisciplines().SingleOrDefault(d => d.id == gr.disciplinesID);
                return singleOrDefault != null && singleOrDefault.course == course && gr.Deleted == false && gr.Wave == 1;
            });
            var stdInGroups = _repository.GetStudentsInGroups().Where(sig => groupsFrstWave.Any(gr => gr.id == sig.groupID) || groups.Any(gr => gr.id == sig.groupID));

            var studentsInGroups = stdInGroups as IList<StudentsInGroups> ?? stdInGroups.ToList();
            var studentsList = students as IList<Students> ?? students.ToList();

            //find std id that not registered
            var notRegisteredStd = (from std in studentsList
                                    join sig in studentsInGroups on std.id equals sig.studentID into loj
                                    from l in loj.DefaultIfEmpty()
                                    where l == null
                                    select std).ToList();


            //get IQueriable that have groupId and std count
            var enumerable = groups as IList<Group> ?? groups.ToList();
            var smallSig = (from gr in enumerable
                            orderby gr.AmountOfStudent descending
                            where gr.Wave == 2 && gr.Deleted == false
                            select new { GroupId = gr.id, Count = gr.AmountOfStudent }).ToList();


            var groupIndexer = 0;

            if (smallSig.Count != 0)
            {
                for (int i = 0; i < notRegisteredStd.Count; i++)
                {
                    if (groupIndexer >= smallSig.Count)
                    {
                        groupIndexer = 0;
                    }
                    _repository.AddStudentInGroups(new StudentsInGroups()
                    {
                        studentID = notRegisteredStd[i].id,
                        groupID = smallSig[groupIndexer].GroupId,
                        DateOfRegister = DateTime.Now
                    });

                    groupIndexer++;
                }
            }
        }

        private void manUpGroupsSecondly(int course)
        {
            var students = _repository.GetStudents().Where(std => std.course == course);
            var groups = _repository.GetGroups().Where(gr =>
            {
                var singleOrDefault = _repository.GetDisciplines().SingleOrDefault(d => d.id == gr.disciplinesID);
                return singleOrDefault != null && singleOrDefault.course == course && gr.Deleted == false && gr.Wave == 2;
            });
            var groupsFrstWave = _repository.GetGroups().Where(gr =>
            {
                var singleOrDefault = _repository.GetDisciplines().SingleOrDefault(d => d.id == gr.disciplinesID);
                return singleOrDefault != null && singleOrDefault.course == course && gr.Deleted == false && gr.Wave == 1;
            });
            var stdInGroups = _repository.GetStudentsInGroups().Where(sig => groupsFrstWave.Any(gr => gr.id == sig.groupID) || groups.Any(gr => gr.id == sig.groupID));

            var studentsInGroups = stdInGroups as IList<StudentsInGroups> ?? stdInGroups.ToList();
            var studentsList = students as IList<Students> ?? students.ToList();

            //find std id that registered only once
            var temp = from std in studentsList
                       join sig in studentsInGroups on std.id equals sig.studentID into loj
                       from l in loj.DefaultIfEmpty()
                       where l != null
                       group std by std.id into grp
                       where grp.Count() == 1
                       select grp.Key;
            //get students by theirs id 
            var onceRegisteredStd = (from std in studentsList
                                     join t in temp on std.id equals t
                                     select std).ToList();

            // get IQueriable that have groupId and std count          
            var grList = groups as IList<Group> ?? groups.ToList();
            var smallSig = (from gr in grList
                            orderby gr.AmountOfStudent descending
                            where gr.Wave == 2 && gr.Deleted == false
                            select new { GroupId = gr.id, Count = gr.AmountOfStudent }).ToList();

            var groupIndexer = 0;

            if (smallSig.Count != 0)
            {
                int i = 0;
                while (onceRegisteredStd.Count != 0)
                {
                    if (groupIndexer >= smallSig.Count)
                    {
                        groupIndexer = 0;
                    }
                    if (studentsInGroups.Where(el => el.groupID == smallSig[groupIndexer].GroupId)
                                        .All(el => el.studentID != onceRegisteredStd[i].id))
                    {
                        _repository.AddStudentInGroups(new StudentsInGroups()
                        {
                            studentID = onceRegisteredStd[i].id,
                            groupID = smallSig[groupIndexer].GroupId,
                            DateOfRegister = DateTime.Now
                        });
                        onceRegisteredStd.Remove(onceRegisteredStd[i]);
                        i = 0;
                        groupIndexer++;
                    }
                    else
                    {
                        if (i < onceRegisteredStd.Count - 1)
                        {
                            i++;
                        }
                        else
                        {
                            groupIndexer++;
                        }
                    }
                }
            }
        }

        public void FillGroupsWithRemainedStd()
        {
            for (int i = 1; i < MaxCourse; i++)
            {
                manUpGroupsFirstly(i);
            }

            for (int i = 1; i < MaxCourse; i++)
            {
                manUpGroupsSecondly(i);
            }
        }

    }
}
