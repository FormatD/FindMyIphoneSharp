

using System;

namespace FindMyIphoneSharp.Models
{
    public class UserInfo
    {
        private readonly String _firstName;
        private readonly String _lastName;

        public UserInfo(String firstName, String lastName)
        {
            _firstName = firstName;
            _lastName = lastName;
        }

        public String GetFirstName()
        {
            return _firstName;
        }

        public String GetLastName()
        {
            return _lastName;
        }

        public override String ToString()
        {
            return "UserInfo{" +
                   "firstName='" + _firstName + '\'' +
                   ", lastName='" + _lastName + '\'' +
                   '}';
        }
    }
}
