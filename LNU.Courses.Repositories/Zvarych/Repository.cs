using System;
using System.Collections.Generic;
using System.Linq;
using Entities;

namespace LNU.Courses.Repositories
{
    public partial class Repository : IRepository
    {
        public IEnumerable<Users> GetUsers()
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                return context.Users.ToList();
            }
        }

        public List<int> GetDisciplinesForSecondWave()
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var groups = context.Group.Where(gr => gr.StudentsInGroups.Count >= 25 && gr.Wave == 1).ToList();
                return groups.Select(item => item.disciplinesID).ToList();
            }
        }
        public IEnumerable<Disciplines> GetDisciplines()
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var disciplines = context.Disciplines.ToList();

                return disciplines;
            }
        }

        public void AddGroup(Group group)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                context.Group.Add(group);

                context.SaveChanges();
            }
        }

        public void UpdateGroup(Group group)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var gr = context.Group.SingleOrDefault(el => el.id == group.id);
                if (gr != null)
                {
                    gr.Deleted = group.Deleted;
                    gr.AmountOfStudent = group.AmountOfStudent;
                    gr.Status = group.Status;
                    gr.Wave = group.Wave;
                    gr.disciplinesID = group.disciplinesID;
                    gr.year = group.year;

                    context.SaveChanges();
                }
            }
        }

        public void DeleteGroups()
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                context.Group.ToList().ForEach(gr => gr.Deleted = true);
                context.SaveChanges();
            }
        }



        #region admins

        public bool DeleteAdmin(string login)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var admin = context.Administrators.SingleOrDefault(st => st.login == login);
                context.Administrators.Remove(admin);
                context.SaveChanges();
                return true;
            }
        }


        public bool AddAdmin(Administrators admin)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                bool valToReturn = true;
                if (context.Administrators.Any(el => el.login == admin.login))
                {
                    valToReturn = false;
                }
                else
                {
                    admin.password = _hashProvider.Encrypt(admin.password);
                    context.Administrators.Add(admin);
                    context.SaveChanges();
                }

                return valToReturn;
            }
        }

        public void UpdateAdmin(Administrators adminAcc)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var admin = context.Administrators.SingleOrDefault(el => el.login == adminAcc.login);
                if (admin != null) admin.roles = adminAcc.roles;

                context.SaveChanges();
            }
        }


        //public Administrators GetAdmin(string login)
        //{
        //    using (var context = new CoursesOfChoiceEntities())
        //    {
        //        var admin = context.Administrators.SingleOrDefault(el => el.login == login);

        //        return admin;
        //    }
        //}

        public IEnumerable<Administrators> GetAdmins()
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var adminList = context.Administrators;

                return adminList.ToList();
            }
        }
        #endregion

        #region students 


        /// <summary>
        /// Get students list that have the same discipline by discipline id
        /// </summary>
        /// <param name="disciplineId"></param>
        /// <param name="wave"></param>
        /// <returns></returns>
        private IEnumerable<Students> GetStudents(int disciplineId, int wave)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var discipline = context.Disciplines.SingleOrDefault(disc => disc.id == disciplineId);
                if (discipline != null)
                {
                    var groupList = discipline.Group;
                    var existingGroups = groupList.SingleOrDefault(gr => gr.Deleted == false && gr.Wave == wave);

                    var stInGroup = existingGroups?.StudentsInGroups;
                    if (stInGroup != null)
                    {
                        var stList =
                            from stInGr in stInGroup
                            join student in context.Students on stInGr.studentID equals student.id
                            where student.Deleted == false
                            select student;

                        return stList.ToList();
                    }

                }

                return new List<Students>();

            }
        }

        /// <summary>
        /// Update student who has id == student.id
        /// </summary>
        /// <param name="student"></param>
        public void UpdateStudent(Students student)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var studentToUpdate = context.Students.SingleOrDefault(st => st.id == student.id);
                if (studentToUpdate != null)
                {
                    studentToUpdate.fio = student.fio;
                    studentToUpdate.course = student.course;
                    studentToUpdate.@group = student.@group;
                    studentToUpdate.AverageMark = student.AverageMark;
                    studentToUpdate.Deleted = student.Deleted;
                }


                context.SaveChanges();
            }
        }
        /// <summary>
        /// Add new student
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public bool AddStudent(Students student)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                bool valToReturn = true;
                if (context.Students.Any(el => el.id == student.id))
                {
                    valToReturn = false;
                }
                else
                {
                    context.Students.Add(student);
                    context.SaveChanges();
                }

                return valToReturn;
            }
        }

        /// <summary>
        /// "Deleting" student ( means make his Deleted attr true)
        /// </summary>
        /// <param name="id">Student id</param>
        /// <returns>State of operation</returns>
        public bool DeleteStudent(string id)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var student = context.Students.SingleOrDefault(st => st.id == id);

                if (student != null && student.Deleted)
                {
                    return false;
                }
                else
                {
                    if (student != null) student.Deleted = true;
                    context.SaveChanges();
                    return true;
                }
            }
        }


        #endregion

        #region disciplines


        /// <summary>
        /// Delete discipline
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteDiscipline(int id)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var discipline = context.Disciplines.SingleOrDefault(disc => disc.id == id);
                if (discipline == null)
                {
                    return false;
                }
                else
                {
                    var singleOrDefault = discipline.Group.SingleOrDefault(gr => gr.Deleted == false && gr.Wave == 1);
                    var groups = discipline.Group.ToList();
                    foreach (var gr in groups)
                    {
                        var sigs = gr.StudentsInGroups;
                        context.Set<StudentsInGroups>().RemoveRange(sigs);
                        context.Set<Group>().Remove(gr);
                    }
                    context.Set<Disciplines>().Remove(discipline);
                    context.SaveChanges();
                    return true;
                }
            }
        }


        /// <summary>
        /// Get discipline which has id == id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Disciplines GetDiscipline(int id)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var disciplines = context.Disciplines.Where(d => d.id == id).ToList();

                return disciplines.SingleOrDefault();
            }
        }

        /// <summary>
        /// Update discipline with values from discipline param
        /// </summary>
        /// <param name="discipline"></param>
        public void UpdateDiscipline(Disciplines discipline)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var disciplineToUpdate = context.Disciplines.SingleOrDefault(disc => disc.id == discipline.id);
                if (disciplineToUpdate != null)
                {
                    disciplineToUpdate.name = discipline.name;
                    disciplineToUpdate.lecturer = discipline.lecturer;
                    disciplineToUpdate.kafedra = discipline.kafedra;
                    disciplineToUpdate.course = discipline.course;
                    disciplineToUpdate.description = discipline.description;
                }


                context.SaveChanges();
            }
        }

        /// <summary>
        /// Add new discipline
        /// </summary>
        /// <param name="discipline"></param>
        /// <returns></returns>
        public bool AddDiscipline(Disciplines discipline)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var valToReturn = true;
                if (context.Disciplines.Any(el => el.name == discipline.name))
                {
                    valToReturn = false;
                }
                else
                {
                    context.Disciplines.Add(discipline);
                    context.SaveChanges();
                }

                return valToReturn;
            }
        }

        #endregion
    }
}