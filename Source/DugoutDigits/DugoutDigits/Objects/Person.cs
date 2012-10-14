using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DugoutDigits.Utilities;

namespace DugoutDigits.Objects
{
    /// <summary>
    /// Object to hold information about a person. This is a base class and all 
    /// other "people" classes must derive from here.
    /// </summary>
    public class Person
    {
        public String firstName { get; set; }   // The first name of the user.
        public String lastName { get; set; }   // The last name of the user.
        public String email { get; set; }   // The email of the user.
        public String imageURL { get; set; }   // The URL of the user's profile picture.
        public DateTime birthday { get; set; }   // The person's birthday.
        public int height { get; set; }   // The height of the person (in inches).
        public int weight { get; set; }   // The weight of the person (in pounds).

        public long ID { get; set; }

        private String password;            // The password for the user.
        private List<Team> teams;           // The teams the user is associated with.

        public Person(String newFirstName,
                      String newLastName,
                      String newEmail,
                      String newImageURL,
                      String newPassword,
                      DateTime newBirthday,
                      int newHeight,
                      int newWeight)
        {
            firstName = newFirstName;
            lastName = newLastName;
            email = newEmail;
            imageURL = newImageURL;
            birthday = newBirthday;
            height = newHeight;
            weight = newWeight;
            password = Utilities.PasswordEncryptor.encrypt_password(newPassword, AppConstants.PASSWORD_KEY);
            teams = new List<Team>();
        }

        public Person(String newFirstName,
              String newLastName,
              String newEmail,
              String newPassword)
            : this(newFirstName, newLastName, newEmail, "http://i.imgur.com/meg9d.png", newPassword, new DateTime(), 0, 0) { }

        public Person(String newFirstName, String newLastName) : this(newFirstName, newLastName, "somerandomemail@someemailclient.com", "password") { }

        public Person() : this("Default", "User") { }

        /// <summary>
        /// Adds the given team to the list of teams this person is associated with.
        /// </summary>
        /// <param name="newTeam">The team to be added.</param>
        public void addTeam(Team newTeam)
        {
            teams.Add(newTeam);
        }

        /// <summary>
        /// Gets the unencrypted password for this user.
        /// </summary>
        /// <returns>The unencrypted password.</returns>
        public String getUnencryptedPassword()
        {
            return Utilities.PasswordEncryptor.decrypt_password(password, AppConstants.PASSWORD_KEY);
        }

        public String getPassword() {
            return password;
        }

        /// <summary>
        /// Encrypts the given passwords and updates the password field.
        /// </summary>
        /// <param name="newPass">The password to encrypt and save.</param>
        public void setUnencryptedPassword(String newPass)
        {
            password = Utilities.PasswordEncryptor.encrypt_password(newPass, AppConstants.PASSWORD_KEY);
        }

        public void setPassword(String newPass) {
            password = newPass;
        }

        /// <summary>
        /// Adds the given position ID to the list of positions.
        /// </summary>
        /// <param name="ID">The ID of the position to add.</param>
        public void addPosition(int ID) {
        }

        /// <summary>
        /// Adds the given position abbreviation to the list of positions. It looks 
        /// up the position ID using the List defined in app constants.
        /// </summary>
        /// <param name="abbr">The abbreviated position to add.</param>
        public void addPosition(String abbr) {
        }

        /// <summary>
        /// Checks if the given position ID is already in the list of positions.
        /// </summary>
        /// <param name="ID">The position ID to check for.</param>
        /// <returns>If the ID is found or not.</returns>
        public bool containsPosition(int ID) {
            return false;
        }

        /// <summary>
        /// Checks if the gien position abbreviation is already in the list of positions.
        /// </summary>
        /// <param name="abbr">The position abbreviation to check for.</param>
        /// <returns>If the abbreviation is found or not.</returns>
        public bool containsPosition(String abbr) {
            return false;
        }

        /// <summary>
        /// Returns the string formated height of the player (F' II").
        /// </summary>
        /// <returns>String formatted height.</returns>
        public String getHeightFormattedString() {
            String feet = ((int)(height / 12)).ToString();
            String inches = (height % 12).ToString();
            return feet + "' " + inches + "\"";
        }
    }
}