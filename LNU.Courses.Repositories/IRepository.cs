using System.Collections.Generic;
using Entities;
using System;
using System.Linq.Expressions;

namespace LNU.Courses.Repositories
{
    public interface IRepository
    {
        IEnumerable<Students> GetStudents(Expression<Func<Students, bool>> expression);
        Students GetStudent(Expression<Func<Students, bool>> expression);
        IEnumerable<Disciplines> GetDisciplines(Expression<Func<Disciplines, bool>> expression);
        IEnumerable<Disciplines> GetUserRegisteredDisc(string login);
        //
        IEnumerable<Users> GetUsers();
        int GetDisciplineWhereRegistered(string login);
        void deleteAmountStudent(int groupID);
        void addAmountStudent(int groupID);
        bool checkRegisteredPhoneNumber(string login);
        bool checkRegisteredEmail(string login);
        IEnumerable<StudentsInGroups> GetStudentsInGroups();
        void AddStudentInGroups(StudentsInGroups sig);
        void DeleteStudentInGroups(StudentsInGroups sig);

        IEnumerable<Students> GetStudents();

        IEnumerable<Group> GetGroups();
        void UpdateGroup(Group group);

        IEnumerable<Disciplines> GetDisciplines();
            //

        List<int> GetDisciplinesForSecondWave();
        void ManUpGroupsForSecondWave();
        bool CheckStudentForRegistered(Students st);
        List<Disciplines> GetDisciplinesSort(string login,int wave);
        void DeleteMyDiscipline(int id, string login);
        IEnumerable<Disciplines> GetD(string login);
        bool CheckRegisteredStudent(string id);
        Group GetGroupByDisciplinesId(int id);
        Group GetGroupByDisciplinesId(int id, int wave);//+wave
        void ChangeStudentPhone(string login, string newPhone);
        void ChangeStudentEMail(string login, string newEMail);
        void ChangeUserPass(string login, string newPass);
        Users UserLogIn(string login, string password);
        Users GetUser(string login);        


        void AddGroup(Group group);

        void DeleteGroups();


        bool DeleteAdmin(string id);

        bool AddAdmin(Administrators admin);

        void UpdateAdmin(Administrators adminAcc);

        bool DeleteDiscipline(int id);

        bool AddDiscipline(Disciplines discipline);
  

        IEnumerable<Administrators> GetAdmins();

        void UpdateStudent(Students student);
    

        bool AddStudent(Students student);

        bool DeleteStudent(string id);

        Disciplines GetDiscipline(int id);

        void UpdateDiscipline(Disciplines discipline);
        void AddLecturer(Lecturer lecturer);
        void UpdateLecturer(Lecturer lecturer);
        bool DeleteLecturer(Lecturer lecturer);
        IEnumerable<Lecturer> GetLecturers();
    }
}
