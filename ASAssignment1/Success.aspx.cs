using System;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.IO;

namespace ASAssignment1
{
    public partial class Success : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        byte[] cc = null;
        byte[] cccvv = null;
        string userID = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check whether user is signed in, if not, redirect to login page
            if (Session["userID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    userID = (string)Session["userID"];
                    displayUserProfile(userID);
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }
        // Method to decrypt the data that is encrypted in the database, such as Credit Card number
        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;

                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrpt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrpt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }
        protected void displayUserProfile(string userID)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Account WHERE Email=@userID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userID", userID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != DBNull.Value)
                        {
                            lbUserID.Text = reader["Email"].ToString();
                        }
                        if (reader["FName"] != DBNull.Value)
                        {
                            lbFName.Text = reader["FName"].ToString();
                        }
                        if (reader["LName"] != DBNull.Value)
                        {
                            lbLName.Text = reader["LName"].ToString();
                        }
                        if (reader["DOB"] != DBNull.Value)
                        {
                            lbDOB.Text = reader["DOB"].ToString();
                        }
                        if (reader["Cc"] != DBNull.Value)
                        {
                            cc = Convert.FromBase64String(reader["Cc"].ToString());
                        }
                        if (reader["CcCVV"] != DBNull.Value)
                        {
                            cccvv = Convert.FromBase64String(reader["CcCVV"].ToString());
                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }
                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }
                    }
                    lbCc.Text = decryptData(cc);
                    lbCcCVV.Text = decryptData(cccvv);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Request.Cookies["AuthToken"].Value = string.Empty;
                Request.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }
    }
}
