using System;
using System.Collections.Generic;
using System.Text;

namespace AirshowAddmin
{
    public class User
    {

        public User(string token, string email, string localId, string refreshToken)
        {
            this.token = token;
            this.email = email;
            this.localId = localId;
            this.refreshToken = refreshToken;
        }
        public string token;

        public string email;

        public string localId;

        public string refreshToken;

        public override string ToString()
        {
            return "Token: " + token + "\nEmail: " + email + "\nLocalID: " + localId + "\nRefresh Token: " + refreshToken;
        }
    }
}
