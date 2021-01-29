using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASAssignment1
{
    public partial class ChangePass : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnChangePass_Click(object sender, EventArgs e)
        {
            string oldPwd = tbOldPass.Text.ToString().Trim();
            string newPwd = tbNewPass.Text.ToString().Trim();
            string userID = HttpUtility.HtmlEncode(tbEmail.Text.ToString().Trim());

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(userID);
            string dbSalt = getDBSalt(userID);

            DateTime minPassAge = getMinPassAge(userID);
            // cantChangePass timespan is to check whether it has been 5 minutes or not.
            TimeSpan cantChangePass = getMinPassTime(minPassAge, DateTime.Now);
            try
            {
                // If user exists in database, log them in
                // Also check whether it has been 5 minutes - if not, they can't change password
                if (cantChangePass <= TimeSpan.Zero)
                {
                    // Check whether user exists in database
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string pwdWithSalt = oldPwd + dbSalt;
                        byte[] hashwWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        string userHash = Convert.ToBase64String(hashwWithSalt);
                        // If old password matches database password
                        if (userHash.Equals(dbHash))
                        {
                            // Generate random "salt"
                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];

                            // Fills array of bytes with a cryptographically strong sequence of random values
                            rng.GetBytes(saltByte);
                            salt = Convert.ToBase64String(saltByte);

                            // Define hashing function
                            SHA512Managed newHashing = new SHA512Managed();

                            string newPwdWithSalt = newPwd + salt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(newPwdWithSalt));

                            finalHash = Convert.ToBase64String(hashWithSalt);

                            RijndaelManaged cipher = new RijndaelManaged();
                            cipher.GenerateKey();
                            Key = cipher.Key;
                            IV = cipher.IV;

                            setNewPass(finalHash, salt, userID);

                            DateTime timeMin = DateTime.Now.AddMinutes(5);
                            DateTime timeMax = DateTime.Now.AddMinutes(15);

                            setMinPassAge(timeMin, userID);
                            setMaxPassAge(timeMax, userID);
                            Response.Redirect("Success.aspx", false);
                        }
                        // If old password does not match the password in the database,
                        else
                        {
                            lbErrorMsg.Text = "Invalid old password!";
                        }
                    }
                    // If user cannot be found,
                    else
                    {
                        lbErrorMsg.Text = "User does not exist in database!";
                    }
                }
                // If 5 minutes have not been up
                else
                {
                    lbErrorMsg.Text = "You need to wait for 5 minutes before you can change your password!";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        protected string getDBHash(string userid)
        {
            string h = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordHash FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;

        }
        protected string getDBSalt(string userid)
        {
            string s = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT PasswordSalt FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                s = reader["PasswordSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }
        private void setNewPass(string newPasswordHash, string newPasswordSalt, string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET PasswordHash=@NEWHASH WHERE Email='" + userid + "'";
            string sql2 = "UPDATE Account SET PasswordSalt=@NEWSALT WHERE Email='" + userid + "'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlCommand cmd2 = new SqlCommand(sql2, connection);
            cmd.Parameters.AddWithValue("@NEWHASH", newPasswordHash);
            cmd2.Parameters.AddWithValue("@NEWSALT", newPasswordSalt);

            cmd.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
        }
        private DateTime getMinPassAge(string userid)
        {
            string t = null;

            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT MinPassAge FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@USERID", userid);

            con.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    t = reader["MinPassAge"].ToString();
                }
            }

            DateTime minPassAge = Convert.ToDateTime(t);
            return minPassAge;
        }
        private DateTime getMaxPassAge(string userid)
        {
            string t = null;

            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT MaxPassAge FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@USERID", userid);

            con.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    t = reader["MaxPassAge"].ToString();
                }
            }

            DateTime maxPassAge = Convert.ToDateTime(t);
            return maxPassAge;
        }
        private void setMinPassAge(DateTime time, string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET MinPassAge='" + time + "' WHERE Email='" + userid + "'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
        private void setMaxPassAge(DateTime time, string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET MaxPassAge='" + time + "' WHERE Email='" + userid + "'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
        private TimeSpan getMinPassTime(DateTime timeStart, DateTime timeEnd)
        {
            TimeSpan timeLeft = timeStart.Subtract(timeEnd);
            return timeLeft;
        }
        private TimeSpan getMaxPassTime(DateTime timeStart, DateTime timeEnd)
        {
            TimeSpan timeLeft = timeStart.Subtract(timeEnd);
            return timeLeft;
        }
    }
}