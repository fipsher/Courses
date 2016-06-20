using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using LNU.Courses.Repositories;

namespace LNU.Courses.BLL.DeadlinesBLL
{
    public class DeadlineManUp
    {
        private readonly IRepository _repository;
        private const int GroupAbstractLimit = 25;
        private const int MaxCourse = 5;

        public DeadlineManUp(IRepository repository)
        {
            _repository = repository;
        }

        public void ManUpGroups()
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

        /// <summary>
        /// Fills groups with stdAmount less than GroupAbstractLimit with std that registered in one group 
        /// </summary>
        private void manUpGroupsSecondly(int course)
        {
            var students = _repository.GetStudents().Where(std => std.course == course);
            var groups = _repository.GetGroups().Where(gr =>
            {
                var singleOrDefault = _repository.GetDisciplines().SingleOrDefault(d => d.id == gr.disciplinesID);
                return singleOrDefault != null && singleOrDefault.course == course && gr.Deleted == false && gr.Wave == 1;
            });
            var stdInGroups = _repository.GetStudentsInGroups().Where(sig => groups.Any(gr => gr.id == sig.groupID));

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
            var smallSig = (from sig in studentsInGroups
                            group sig by sig.groupID into gSig
                            orderby gSig.Count() descending
                            select new { GroupId = gSig.Key, Count = gSig.Count() }).ToList();

            smallSig = smallSig.Where(si =>
            {
                var firstOrDefault = groups.FirstOrDefault(gr => gr.id == si.GroupId);
                return firstOrDefault != null && firstOrDefault.Deleted == false;
            }).ToList();

            foreach (var t in smallSig)
            {
                if (t.Count < GroupAbstractLimit)
                {
                    // students that's need to add to fill group
                    var stdAmountToAdd = GroupAbstractLimit - t.Count;

                    if (onceRegisteredStd.Count >= stdAmountToAdd)
                    {
                        //var stdToAdd = notRegisteredStd.GetRange(0, stdAmountToAdd);
                        int indexStd = 0;
                        int stdAdded = 0;
                        //fill this group with not registered student
                        while (stdAdded < stdAmountToAdd && indexStd < onceRegisteredStd.Count)
                        {
                            if (studentsInGroups.Where(el => el.groupID == t.GroupId)
                                                .All(el => el.studentID != onceRegisteredStd[indexStd].id))
                            {
                                _repository.AddStudentInGroups(new StudentsInGroups()
                                {
                                    studentID = onceRegisteredStd[indexStd].id,
                                    groupID = t.GroupId,
                                    DateOfRegister = DateTime.Now
                                });
                                onceRegisteredStd.Remove(onceRegisteredStd[indexStd]);
                                stdAdded++;
                            }
                            else
                            {
                                indexStd++;
                            }
                        }
                    }
                    else
                    {
                        var sigToRemove = studentsInGroups.Where(sig => sig.groupID == t.GroupId).ToList();
                        foreach (var sigToRem in sigToRemove)
                        {
                            _repository.DeleteStudentInGroups(sigToRem);
                        }
                    }
                }
                else
                {
                    var group = _repository.GetGroups().SingleOrDefault(el => el.id == t.GroupId);
                    if (@group != null && @group.AmountOfStudent >= 25)
                    {
                        @group.Status = true;
                        _repository.UpdateGroup(@group);

                        _repository.AddGroup(new Group()
                        {
                            disciplinesID = @group.disciplinesID,
                            Deleted = false,
                            AmountOfStudent = 0,
                            Status = false,
                            Wave = 2,
                            year = DateTime.Now.Year
                        });
                    }
                }
            }
            onceRegisteredStd.ForEach(std => std.locked = true);

            foreach (var std in onceRegisteredStd)
            {
                _repository.UpdateStudent(std);
            }
        }

        /// <summary>
        /// Fills groups whith stdAmount less than GroupAbstractLimit with std that do not register
        /// </summary>
        private void manUpGroupsFirstly(int course)
        {
            var students = _repository.GetStudents().Where(std => std.course == course);
            var groups = _repository.GetGroups().Where(gr =>
            {
                var singleOrDefault = _repository.GetDisciplines().SingleOrDefault(d => d.id == gr.disciplinesID);
                return singleOrDefault != null && singleOrDefault.course == course && gr.Deleted == false && gr.Wave == 1;
            });
            var stdInGroups = _repository.GetStudentsInGroups().Where(sig => groups.Any(gr => gr.id == sig.groupID));

            var studentsInGroups = stdInGroups as IList<StudentsInGroups> ?? stdInGroups.ToList();
            var studentsList = students as IList<Students> ?? students.ToList();

            //find std id that registered only once
            var notRegisteredStd = (from std in studentsList
                                    join sig in studentsInGroups on std.id equals sig.studentID into loj
                                    from l in loj.DefaultIfEmpty()
                                    where l == null
                                    select std).ToList();


            //get IQueriable that have groupId and std count
            var smallSig = (from sig in studentsInGroups
                            group sig by sig.groupID into gSig
                            orderby gSig.Count() descending
                            select new { GroupId = gSig.Key, Count = gSig.Count() }).ToList();

            smallSig = smallSig.Where(si =>
            {
                var firstOrDefault = groups.FirstOrDefault(gr => gr.id == si.GroupId);
                return firstOrDefault != null && firstOrDefault.Deleted == false;
            }).ToList();

            foreach (var t in smallSig)
            {
                if (t.Count < GroupAbstractLimit)
                {
                    // students that's need to add to fill group
                    var stdAmountToAdd = GroupAbstractLimit - t.Count;

                    if (notRegisteredStd.Count >= stdAmountToAdd)
                    {
                        //var stdToAdd = notRegisteredStd.GetRange(0, stdAmountToAdd);
                        int stdAdded = 0;
                        //fill this group with not registered student
                        while (stdAdded < stdAmountToAdd)
                        {
                            _repository.AddStudentInGroups(new StudentsInGroups()
                            {
                                studentID = notRegisteredStd.First().id,
                                groupID = t.GroupId,
                                DateOfRegister = DateTime.Now
                            });
                            notRegisteredStd.Remove(notRegisteredStd.First());
                            stdAdded++;
                        }
                    }
                }
            }
        }
    }
}

