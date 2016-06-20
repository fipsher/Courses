using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using LNU.Courses.BLL.RepoBLL;
using LNU.Courses.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LNU.Courses.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private Mock<IRepository> _repoMock;

        [TestInitialize]
        public void Initialize()
        {
            _repoMock = new Mock<IRepository>();

            _repoMock.Setup(el => el.GetStudents()).Returns(_students);
            _repoMock.Setup(el => el.GetStudentsInGroups()).Returns(new List<StudentsInGroups>());
            _repoMock.Setup(el => el.GetUsers()).Returns(_users);
            _repoMock.Setup(el => el.GetGroups()).Returns(_groups);
            _repoMock.Setup(el => el.GetDisciplines()).Returns(_disciplines);
            _repoMock.Setup(el => el.AddGroup(It.IsAny<Group>())).Callback<Group>(gr => _groups.Add(gr));



            initialize();
        }

        [TestMethod]
        public void TestStdSearching()
        {
            var repoBl = new RepositoryBL(_repoMock.Object);
            for (int i = 0; i < 100; i++)
            {

                string fio = $"Surname Name Parent {i}";
                var student = repoBl.GetStudents(i.ToString()).ToList()[0];

                Assert.IsNotNull(student);
                Assert.AreEqual(student.fio, fio);
            }

        }

        [TestMethod]
        public void TetsStdEmails1()
        {
           
            var repoBl = new RepositoryBL(_repoMock.Object);

            var eMails = repoBl.GetStudentEmails().ToList();
            Assert.IsTrue(eMails.Count == 100);
        }

        [TestMethod]
        public void TetsStdEmails2()
        {
            var repoBl = new RepositoryBL(_repoMock.Object);

            var eMails = repoBl.GetStdEmailsForSecondWay().ToList();
            Assert.IsTrue(eMails.Count == 100);
        }

        [TestMethod]
        public void TestGroups()
        {
            var repoBl = new RepositoryBL(_repoMock.Object);

            repoBl.CreateNewGroups(2);
            var newGr = _groups.Where(el => el.Wave == 2).ToList();


            Assert.IsTrue(newGr.Count > 0);
        }

        private void initialize()
        {
            for (int i = 0; i < 100; i++)
            {
                _users.Add(new Users()
                {
                    login = i.ToString(),
                    password = i.ToString(),
                });

                _students.Add(new Students()
                {
                    id = i.ToString(),
                    fio = $"Surname Name Parent {i}",
                    course = 3,
                    Deleted = false,
                    locked = false,
                    AverageMark = 4,
                    eMail = $"email{i}",
                    phoneNumber = $"phone{i}"
                });

            }

            for (int i = 0; i < 5; i++)
            {
                _disciplines.Add(new Disciplines()
                {
                    id = i,
                    course = 3,
                    description = $"desc{i}",
                    kafedra = $"kaf{i}",
                    name = $"discName{i}"
                });

                _groups.Add(new Group()
                {
                    id = i,
                    AmountOfStudent = 0,
                    disciplinesID = 0,
                    year = DateTime.Now.Year,
                    Deleted = false,
                    Status = false,
                    Wave = 1
                });
            }
        }

        private List<Students> _students = new List<Students>();
        private List<Users> _users = new List<Users>();
        private List<Group> _groups = new List<Group>();
        private List<Disciplines> _disciplines = new List<Disciplines>();
    }
}
