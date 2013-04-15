using System;
using DugoutDigits.Objects;
using DugoutDigits.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestAccessor {
    [TestClass]
    public class AccountManagement {
        [TestMethod]
        public void Test_CheckEmail_Found() {
            DBAccessor dba = new DBAccessor();
            bool takenEmail = dba.CheckEmail(TestConstants.CHECKEMAIL_TAKENEMAIL);
            Assert.AreEqual(false, takenEmail);
        }

        [TestMethod]
        public void Test_CheckEmail_NotFound() {
            DBAccessor dba = new DBAccessor();
            bool openEmail = dba.CheckEmail(TestConstants.CHECKEMAIL_OPENEMAIL);
            Assert.AreEqual(true, openEmail);
        }

        [TestMethod]
        public void Test_CheckLoginCredentials_Valid() {
            DBAccessor dba = new DBAccessor();
            LogonResponse response = dba.CheckLoginCredentials(TestConstants.CHECKLOGINCREDENTIALS_VALIDEMAIL, TestConstants.CHECKLOGINCREDENTIALS_VALIDPASS);
            Assert.AreEqual((int) LogonResults.SUCCESS, response.success);
            Assert.AreEqual(TestConstants.CHECKLOGINCREDENTIALS_VALIDEMAIL, response.user.email);
            Assert.AreEqual(TestConstants.CHECKLOGINCREDENTIALS_FIRSTNAME, response.user.firstName);
            Assert.AreEqual(TestConstants.CHECKLOGINCREDENTIALS_LASTNAME, response.user.lastName);
        }

        [TestMethod]
        public void Test_CheckLoginCredentials_BadUsername() {
            DBAccessor dba = new DBAccessor();
            LogonResponse response = dba.CheckLoginCredentials(TestConstants.CHECKLOGINCREDENTIALS_INVALIDEMAIL, "Don't Care");
            Assert.AreEqual((int) LogonResults.USERNOTFOUND, response.success);
        }

        [TestMethod]
        public void Test_CheckLoginCredentials_BadPassword() {
            DBAccessor dba = new DBAccessor();
            LogonResponse response = dba.CheckLoginCredentials(TestConstants.CHECKLOGINCREDENTIALS_VALIDEMAIL, TestConstants.CHECKLOGINCREDENTIALS_INVALIDPASS);
            Assert.AreEqual((int) LogonResults.PASSWORDMISMATCH, response.success);
        }

        [TestMethod]
        public void Test_GetPersonInformation_Found() {
            DBAccessor dba = new DBAccessor();
            Person person = dba.GetPersonInformation(TestConstants.GETPERSONINFORMATION_VALIDEMAIL);
            Assert.AreEqual(TestConstants.GETPERSONINFORMATION_VALIDEMAIL, person.email);
            Assert.AreEqual(TestConstants.GETPERSONINFORMATION_PASSWORD, person.getUnencryptedPassword());
            Assert.AreEqual(TestConstants.GETPERSONINFORMATION_FIRSTNAME, person.firstName);
            Assert.AreEqual(TestConstants.GETPERSONINFORMATION_LASTNAME, person.lastName);
            Assert.AreEqual(TestConstants.GETPERSONINFORMATION_USERID, person.ID);
            Assert.AreEqual(TestConstants.GETPERSONINFORMATION_COACHENABLED, person.permissions.coachEnabled);
        }

        [TestMethod]
        public void Test_GetPersonInformation_NotFound() {
            DBAccessor dba = new DBAccessor();
            Person person = dba.GetPersonInformation(TestConstants.GETPERSONINFORMATION_INVALIDEMAIL);
            Assert.AreEqual(null, person);
        }
    }
}
