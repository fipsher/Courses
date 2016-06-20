using System.Collections.Generic;
using Entities;

namespace LNU.Courses.Repositories
{
    public interface IRepository
    {
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
        IEnumerable<Disciplines> GetDisciplinesSort(string login,int wave);
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

        //IEnumerable<string> GetStdEmailsForSecondWay();

        void AddGroup(Group group);

        //void CreateNewGroups(int wave);

        void DeleteGroups();

        //IEnumerable<string> GetStudentEmails();

        //void ChangeAdminPass(string login, string newPass);

        //Administrators AdminLogIn(string login, string password);

        bool DeleteAdmin(string id);

        bool AddAdmin(Administrators admin);

        void UpdateAdmin(Administrators adminAcc);

        bool DeleteDiscipline(int id);

        bool AddDiscipline(Disciplines discipline);

        //Administrators GetAdmin(string login);

        IEnumerable<Administrators> GetAdmins();

        //IEnumerable<Students> GetStudents(string partOfName);

        //IEnumerable<Students> GetStudents(int disciplineId, int wave);

        void UpdateStudent(Students student);

        //Students GetStudentById(string id);//

        bool AddStudent(Students student);

        bool DeleteStudent(string id);

        //IEnumerable<Disciplines> GetDisciplines(string name);

        Disciplines GetDiscipline(int id);

        void UpdateDiscipline(Disciplines discipline);
    }
}
