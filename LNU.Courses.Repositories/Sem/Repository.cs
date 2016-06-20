using System;
using System.Collections.Generic;
using System.Linq;
using Entities;

namespace LNU.Courses.Repositories
{
    public partial class Repository : IRepository
    {
        readonly IHashProvider _hashProvider = new HashProvider();
        public int GetDisciplineWhereRegistered(string login)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var sig = context.StudentsInGroups.Where(s => s.studentID == login).ToList();

                if (sig.Count == 1)
                {
                    var g = sig[0].Group;
                    return g.disciplinesID;
                }
                else
                    return 0;
            }
        }
        public void addAmountStudent(int groupID)
        {            
            using (var context = new CoursesOfChoiceEntities())
            {
                var gr = context.Group.SingleOrDefault(g => g.id == groupID);
                gr.AmountOfStudent++;
                context.SaveChanges();

            }
        }
        public void deleteAmountStudent(int groupID)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var gr = context.Group.SingleOrDefault(g => g.id == groupID);
                gr.AmountOfStudent--;
                context.SaveChanges();

            }
        }
        public bool checkRegisteredPhoneNumber(string login)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                if (login != "")
                {
                    var stud = context.Students.Where(st => st.id == login).SingleOrDefault();

                    if (stud.phoneNumber == "")
                        return false;
                    else
                        return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool checkRegisteredEmail(string login)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                if (login != "")
                {
                    var stud = context.Students.Where(st => st.id == login).SingleOrDefault();
                    if (stud.eMail == "")
                        return false;
                    else
                        return true;
                }
                else
                {
                    return false;
                }

            }
        }
        public void ManUpGroupsForSecondWave()
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var notRegisteredStd = (from std in context.Students
                                        join sig in context.StudentsInGroups on std.id equals sig.studentID into loj
                                        from l in loj.DefaultIfEmpty()
                                        where l == null
                                        orderby std.AverageMark descending
                                        select std).ToList();

                // get IQueriable that have groupId and std count
                // all groups that have less then 25 std              
                var smallSig = (from sig in context.StudentsInGroups
                                group sig by sig.groupID into gSig
                                //where gSig.Count() < 25
                                // && context.Group.Where(gr => gr.id == gSig.Key && gr.Wave == wave).FirstOrDefault().Deleted == false
                                orderby gSig.Count() descending
                                select new { GroupId = gSig.Key, Count = gSig.Count() }).ToList();
                smallSig = smallSig.Where(si =>
                {
                    var firstOrDefault = context.Group.FirstOrDefault(gr => gr.id == si.GroupId);
                    return firstOrDefault != null && firstOrDefault.Deleted == false;
                }).ToList();

                for (var i = 0; i < smallSig.Count; i++)
                {
                    if (smallSig[i].Count < 25)
                    {
                        // students that's need to add to fill group
                        int stdAmountToAdd = 25 - smallSig[i].Count;
                        
                            var stdToAdd = notRegisteredStd.GetRange(0, stdAmountToAdd);

                            //fill this group with not registered student
                            for (int j = 0; j < stdAmountToAdd; j++)
                            {
                                notRegisteredStd.Remove(stdToAdd[j]);

                                context.StudentsInGroups.Add(new StudentsInGroups()
                                {
                                    studentID = stdToAdd[j].id,
                                    groupID = smallSig[i].GroupId,
                                    DateOfRegister = DateTime.Now
                                });
                            }
                    }
                }
                
                context.SaveChanges();
            }
        }

        public bool CheckStudentForRegistered(Students st)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var listSig = context.StudentsInGroups.ToList();
                int k = listSig.Count(item => item.studentID == st.id);
                if (k == 2)
                    return true;
                else
                    return false;
            }
        }

        public IEnumerable<StudentsInGroups> GetStudentsInGroups()
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var sig = context.StudentsInGroups.Where(el => el.Students.Deleted == false).ToList();

                return sig;
            }
        }
        //!!!!!!!!!!!!!!!!!!!!
        public void AddStudentInGroups(StudentsInGroups sig)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                context.StudentsInGroups.Add(sig);
                addAmountStudent(sig.groupID);
                context.SaveChanges();
            }
        }

        public void DeleteStudentInGroups(StudentsInGroups sig)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var elToRemove = context.Set<StudentsInGroups>().SingleOrDefault(el => el.id == sig.id);

                context.Set<StudentsInGroups>().Remove(elToRemove);
                deleteAmountStudent(sig.groupID);
                context.SaveChanges();
            }
        }

        public IEnumerable<Students> GetStudents()
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var students = context.Students.ToList();

                return students;
            }
        }

        public IEnumerable<Group> GetGroups()
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var groups = context.Group.Where(el => el.Deleted == false).ToList();

                return groups;
            }
        }
        
        #region user
        public void ChangeUserPass(string login, string newPass)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var userAcc = context.Users.SingleOrDefault(acc => acc.login == login);
                if (userAcc != null) userAcc.password = _hashProvider.Encrypt(newPass);
                context.SaveChanges();
            }
        }
        public Users UserLogIn(string login, string password)
        {

            //password = _hashProvider.Encrypt(password);
            using (var context = new CoursesOfChoiceEntities())
            {
                Users user = context.Users.SingleOrDefault(el => el.login == login && el.password == password);

                return user;
            }
        }
        public Users GetUser(string login)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var user = context.Users.SingleOrDefault(el => el.login == login);

                return user;
            }
        }
        #endregion
        #region Student
        public bool CheckRegisteredStudent(string id)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var student = context.StudentsInGroups.SingleOrDefault(st => st.studentID == id);
                if (student == null)
                    return false;
                else
                    return true;
            }

        }

        public void ChangeStudentEMail(string login, string newEMail)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var studentAcc = context.Students.SingleOrDefault(acc => acc.id == login);
                if (studentAcc != null) studentAcc.eMail = newEMail;
                context.SaveChanges();
            }
        }
        public void ChangeStudentPhone(string login, string newPhone)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var studentAcc = context.Students.SingleOrDefault(acc => acc.id == login);
                if (studentAcc != null) studentAcc.phoneNumber = newPhone;
                context.SaveChanges();
            }
        }

        #endregion
        #region Discipline
        public IEnumerable<Disciplines> GetDisciplinesSort(string login,int wave)
        {
            //tyt
            using (var context = new CoursesOfChoiceEntities())
            {
                var student = GetStudents().SingleOrDefault(std => std.id == login);
                var disciplines = context.Disciplines.Where(el => el.course == student.course).ToList();
                var countStud = disciplines.Select(d => GetStudents(d.id, wave).Count()).ToList();//
                for (int i = 0; i < countStud.Count; i++)
                {
                    var group = disciplines[i].Group.ToList();
                    foreach (var item in group)
                    {
                        countStud[i] = item.StudentsInGroups.Count();
                    }
                }
                for (int i = 1; i < countStud.Count; i++)
                {
                    for (int j = 0; j < countStud.Count - i; j++)
                    {
                        if (countStud[j] < countStud[j + 1])
                        {
                            var temp = countStud[j + 1];
                            countStud[j + 1] = countStud[j];
                            countStud[j] = temp;

                            var discTemp = disciplines[j + 1];
                            disciplines[j + 1] = disciplines[j];
                            disciplines[j] = discTemp;
                        }
                    }
                }

                return disciplines;
            }
        }

        #endregion
        #region Group
        public Group GetGroupByDisciplinesId(int id)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var group = context.Group.Where(el => el.disciplinesID == id && el.Deleted == false).ToList();
                return group.SingleOrDefault();
            }
        }
        public Group GetGroupByDisciplinesId(int id, int wave)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var group = context.Group.SingleOrDefault(el => el.disciplinesID == id && el.Wave == wave && el.Deleted == false);
                return group;
            }
        }
        #endregion
        public IEnumerable<Disciplines> GetD(string login)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var sTinGroup = context.StudentsInGroups.Where(gr => gr.studentID == login);
                var groups = from gr in context.Group
                             join sig in sTinGroup on gr.id equals sig.groupID
                             select gr;
                var discplines = (from gr in groups
                                  join dic in context.Disciplines on gr.disciplinesID equals dic.id
                                  select dic).ToList();



                return discplines;
            }
        }

        public void DeleteMyDiscipline(int id, string login)
        {
            using (var context = new CoursesOfChoiceEntities())
            {
                var sig = context.StudentsInGroups.Where(st => st.studentID == login ).ToList();
                foreach (var item in sig)
                {
                    if (item.Group.Disciplines.id == id)
                    {
                        context.StudentsInGroups.Remove(item);
                        context.SaveChanges();
                    }
                }
            }
        }


    }
}
