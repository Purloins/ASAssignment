using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace ASAssignment1
{
    public partial class Login : System.Web.UI.Page
    {
        static int attemptCount = 0;
        static int attemptLeft = 0;
        public string success { get; set; }
        public List<string> ErrorMessage { get; set; }
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try {
                // SQL Connection Strings to retrieve data from database
                SqlConnection connection = new SqlConnection(MYDBConnectionString);
                string sql = "SELECT * FROM Account WHERE Email=@0";

                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@0", tbEmail.Text);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);

                // Since every person will have its unique ID, then there will only be one row.
                // Use this information from this one row to display on the website.
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string status = getLockStatus(HttpUtility.HtmlEncode(tbEmail.Text.ToString().Trim()));
                    // Set booStatus (which will be LockoutStatus effectively) to status ^
                    bool booStatus = bool.Parse(status);
                    // Check if Captcha is valid, if not, parse error
                    if (ValidateCaptcha())
                    {
                        // Login code
                        string pwd = HttpUtility.HtmlEncode(tbPass.Text.ToString().Trim());
                        string userID = HttpUtility.HtmlEncode(tbEmail.Text.ToString().Trim());

                        SHA512Managed hashing = new SHA512Managed();
                        string dbHash = getDBHash(userID);
                        string dbSalt = getDBSalt(userID);

                        try
                        {
                            // If user exists in database, log them in
                            if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                            {
                                string pwdWithSalt = pwd + dbSalt;
                                byte[] hashwWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                string userHash = Convert.ToBase64String(hashwWithSalt);
                                // If LockoutStatus is false, they are allowed to log in
                                if (booStatus == false)
                                {
                                    DateTime timeStart = DateTime.Now;
                                    DateTime timeEnd = getTimeEnd(userID);
                                    TimeSpan remainTimeLeft = getRemainingTime(timeEnd, timeStart);
                                    // If the user has already waited 10 minutes, set the Status to false
                                    if (remainTimeLeft <= TimeSpan.Zero)
                                    {
                                        setLockStatusFalse(tbEmail.Text);
                                    }
                                    // If password matches database password
                                    if (userHash.Equals(dbHash))
                                    {

                                        Session["UserID"] = userID;

                                        // Create a new GUID and save into the session
                                        string guid = Guid.NewGuid().ToString();
                                        Session["AuthToken"] = guid;

                                        // Create a new cookie with this GUID value
                                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                        Response.Redirect("Success.aspx", false);
                                    }
                                    else
                                    {
                                        // Check for 3 login attempts
                                        // If number of attempts reaches 3, lock the user out
                                        if (attemptCount == 3)
                                        {
                                            // Once attempt count reaches 3, lock the account and reset AttemptCount back to 0.
                                            // I have set the timer to 10 minutes.
                                            setLockStatus(tbEmail.Text);
                                            attemptCount = 0;

                                            DateTime timeNow = DateTime.Now;
                                            DateTime timerLOL = DateTime.Now.AddMinutes(10);
                                            setLockTime(timeNow, userID);
                                            setLockEnd(timerLOL, userID);
                                            TimeSpan remainTime = getRemainingTime(timerLOL, timeNow);
                                            lbErrorMsg.Text = "Your account has been temporarily locked due to three invalid login attempts.<br> Time left: " + remainTime;
                                        }
                                        // If account is already locked out,
                                        else if (booStatus == true)
                                        {
                                            lbErrorMsg.Text = "Your account has been locked out temporarily.";
                                        }
                                        // If username or password is wrong,
                                        else
                                        {
                                            attemptCount = attemptCount + 1;
                                            attemptLeft = 3 - attemptCount;
                                            lbErrorMsg.Text = "Wrong username or password. <br>" + "Total number of attempts left: " + attemptLeft;
                                        }
                                    }
                                }
                                // If lock status of account is true,
                                else
                                {
                                    DateTime timeStart = DateTime.Now;
                                    DateTime timeEnd = getTimeEnd(userID);
                                    TimeSpan remainTime = getRemainingTime(timeEnd, timeStart);
                                    lbErrorMsg.Text = "Your account has been locked out temporarily. <br> Time left: " + remainTime;

                                    if (remainTime <= TimeSpan.Zero)
                                    {
                                        setLockStatusFalse(tbEmail.Text);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.ToString());
                        }
                        finally { }
                    }
                }
                else
                {
                    lbErrorMsg.Text = "Invalid username or password.";
                }
            }
            catch (SqlException ex)
            {
                lbErrorMsg.Text = "Error!";
                throw new Exception(ex.ToString());
            }
        }
        private string getLockStatus(string userid)
        {
            string s = null;

            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT LockStatus FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@USERID", userid);

            con.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    s = reader["LockStatus"].ToString();
                }
            }
            return s;
        }
        private void setLockTime(DateTime time, string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET LockoutTime='" + time + "' WHERE Email='" + userid + "'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
        private void setLockEnd(DateTime time, string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET LockoutEndTime='" + time + "' WHERE Email='" + userid + "'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
        private TimeSpan getRemainingTime(DateTime timeStart, DateTime timeEnd)
        {
            TimeSpan timeLeft = timeStart.Subtract(timeEnd);
            return timeLeft;
        }

        private DateTime getTimeEnd(string userid)
        {
            string t = null;

            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT LockoutEndTime FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, con);
            command.Parameters.AddWithValue("@USERID", userid);

            con.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    t = reader["LockoutEndTime"].ToString();
                }
            }

            DateTime timeEnd = Convert.ToDateTime(t);
            return timeEnd;
        }
        private void setLockStatus(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET LockStatus='true' WHERE Email='" + email + "'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
        private void setLockStatusFalse(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET LockStatus='false' WHERE Email='" + email + "'";

            connection.Open();
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
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
        public bool ValidateCaptcha()
        {
            bool result = true;
            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LfkFDEaAAAAALIH6zt7sAJZv3fgtHOCyiN3bGXE &response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        Login jsonObject = js.Deserialize<Login>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}