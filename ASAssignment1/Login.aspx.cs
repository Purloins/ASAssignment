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
        public string success { get; set; }
        public List<string> ErrorMessage { get; set; }
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {

            // SQL Connection Strings to retrieve data from database
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Account WHERE Email='" + tbEmail.Text + "'";

            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds);

            // Since every person will have its unique ID, then there will only be one row.
            // Use this information from this one row to display on the website.
            if (ds.Tables[0].Rows.Count > 0) { 
                string status = ds.Tables[0].Rows[0]["LockStatus"].ToString();

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
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashwWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashwWithSalt);

                            if (status == "false")
                            {
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
                                    lbErrorMsg.Text = "Wrong username or password." + attemptCount;
                                    attemptCount = attemptCount + 1;

                                    // Check for 3 login attempts
                                    // If number of attempts reaches 3, lock the user out
                                    if (attemptCount == 3)
                                    {
                                        lbErrorMsg.Text = "Your account has been temporarily locked due to three invalid login attempts.";
                                        setLockStatus(tbEmail.Text);
                                        attemptCount = 0;
                                    }
                                    else if (status == "true")
                                    {
                                        lbErrorMsg.Text = "Your account has been locked out temporarily.";
                                    }
                                    
                                }
                            }
                            else
                            {
                                lbErrorMsg.Text = "Your account has been locked out temporarily.";
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
        private void setLockStatus(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "UPDATE Account SET LockStatus='true' WHERE Email='" + email + "'";

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
                    using (StreamReader readStream = new StreamReader (wResponse.GetResponseStream()))
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