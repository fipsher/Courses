using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using LNU.Courses.Repositories;

namespace LNU.Courses.BLL.RepoBLL
{
    public class RepositoryBL
    {
        private readonly IRepository _repository;
        private readonly HashProvider _hashProvider;

        public RepositoryBL(IRepository repository)
        {
            _repository = repository;
            _hashProvider = new HashProvider();
        }



        /// <summary>
        /// Get list of Disciplines filtered by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Disciplines> GetDisciplines(string name)
        {
            var temp = _repository.GetDisciplines();
            var disciplines = temp as IList<Disciplines> ?? temp.ToList();

            var discipline = disciplines.Where(el => el.name.Contains(name));
            return discipline.ToList();

        }

        /// <summary>
        /// get student by his Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Students GetStudentById(string id)
        {
            var students = _repository.GetStudents();
            var student = students.SingleOrDefault(st => st.id == id);
            return student;

        }

        /// <summary>
        /// get student whose name contains partOfName param
        /// </summary>
        /// <param name="partOfName"></param>
        /// <returns></returns>
        public IEnumerable<Students> GetStudents(string partOfName)
        {

            var students = _repository.GetStudents().Where(el => el.fio.Contains(partOfName));
            return students.ToList();

        }

        /// <summary>
        /// Get students list that have the same discipline by discipline id
        /// </summary>
        /// <param name="disciplineId"></param>
        /// <param name="wave"></param>
        /// <returns></returns>
        public IEnumerable<Students> GetStudents(int disciplineId, int wave)
        {

            var discipline = _repository.GetDisciplines().SingleOrDefault(disc => disc.id == disciplineId);
            if (discipline != null)
            {
                var groupList = _repository.GetGroups().Where(gr => gr.disciplinesID == disciplineId);
                var existingGroups = groupList.SingleOrDefault(gr => gr.Deleted == false && gr.Wave == wave);

                var stInGroup = _repository.GetStudentsInGroups().Where(sig => existingGroups != null && sig.groupID == existingGroups.id).ToList();

                var stList =
                    from stInGr in stInGroup
                    join student in _repository.GetStudents() on stInGr.studentID equals student.id
                    where student.Deleted == false
                    select student;

                return stList.ToList();

            }

            return new List<Students>();
        }

        public Administrators AdminLogIn(string login, string password)
        {
            password = _hashProvider.Encrypt(password);

            var admin = _repository.GetAdmins().SingleOrDefault(el => el.login == login && el.password == password);

            return admin;
        }

        public Administrators GetAdmin(string login)
        {

            var admin = _repository.GetAdmins().SingleOrDefault(el => el.login == login);

            return admin;

        }

        public void CreateNewGroups(int wave)
        {

            var disciplines = _repository.GetDisciplines().ToList();

            for (var i = 0; i < disciplines.Count; i++)
            {
                _repository.AddGroup(new Group()
                {
                    disciplinesID = disciplines[i].id,
                    year = DateTime.Now.Year,
                    Status = false,
                    Deleted = false,
                    Wave = wave
                });
            }
        }

        public IEnumerable<string> GetStudentEmails()
        {
            var students = _repository.GetStudents().Where(student => student.fio.Contains(" ") && student.Deleted == false && !string.IsNullOrEmpty(student.eMail));

            var result =
                from student in students
                select student.eMail;
            return result.ToList();
        }


        public void ChangeAdminPass(string login, string newPass)
        {

            var adminAcc = _repository.GetAdmins().SingleOrDefault(acc => acc.login == login);
            if (adminAcc != null) adminAcc.password = _hashProvider.Encrypt(newPass);

            _repository.UpdateAdmin(adminAcc);
        }

        public IEnumerable<string> GetStdEmailsForSecondWay()
        {

            var studentsList = _repository.GetStudents().ToList();
            var stInGr = _repository.GetStudentsInGroups().ToList();

            var eMails = (from std in studentsList
                          where std.locked == false
                          join sig in stInGr on std.id equals sig.studentID into lj
                          from subSig in lj.DefaultIfEmpty()
                          where subSig == null
                          select std.eMail).Where(el => el != null).ToList();

            var temp = from std in studentsList
                       join sig in stInGr on std.id equals sig.studentID into loj
                       from l in loj.DefaultIfEmpty()
                       where l != null
                       group std by std.id into grp
                       where grp.Count() == 1
                       select grp.Key;

            var onceRegisteredStdMails = (from std in studentsList
                                          join t in temp on std.id equals t
                                          select std.eMail).ToList();

            eMails.AddRange(onceRegisteredStdMails.Where(el => el != null));
            return eMails;
        }


   
    }
}
