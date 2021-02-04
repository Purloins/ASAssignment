using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

namespace ASAssignment1
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        public string success { get; set; }
        public List<string> ErrorMessage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check whether user is signed in, if not, redirect to login page
            if (Session["userID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("AlreadyLoggedIn.aspx", false);
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                string pwd = tbPass.Text.ToString().Trim();
                string cfmPwd = tbCfmPass.Text.ToString().Trim();
                // Check whether Confirm Password is same as Password
                if (pwd != cfmPwd)
                {
                    Response.Redirect("Registration.aspx", false);
                }

                // Creating the account
                else
                {
                    // Client-side verificaiton for password checking
                    int score = checkPwd(tbPass.Text);
                    string status = "";
                    switch (score)
                    {
                        case 1:
                            status = "Very Weak";
                            break;
                        case 2:
                            status = "Weak";
                            break;
                        case 3:
                            status = "Medium";
                            break;
                        case 4:
                            status = "Strong";
                            break;
                        case 5:
                            status = "Excellent";
                            break;
                        default:
                            break;
                    }
                    lbPwrdChck.Text = "Status: " + status;

                    // Generate random "salt"
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];

                    // Fills array of bytes with a cryptographically strong sequence of random values
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);

                    // Define hashing function
                    SHA512Managed hashing = new SHA512Managed();

                    string pwdWithSalt = pwd + salt;
                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                    finalHash = Convert.ToBase64String(hashWithSalt);

                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;

                    // Call createAccount() method
                    createAccount();

                    // Redirect to login page
                    Response.Redirect("Login.aspx");
                }
            }
        }
        public void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES(@FName,@LName,@Cc,@CcCVV,@Email,@DOB,@DateTimeRegistered,@IV,@Key)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            // I use two tables so it's easier to seperate password and user information.
                            // First is user information.
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FName", HttpUtility.HtmlEncode(tbFName.Text.Trim()));
                            cmd.Parameters.AddWithValue("@LName", HttpUtility.HtmlEncode(tbLName.Text.Trim()));
                            // Credit card information is encrypted
                            cmd.Parameters.AddWithValue("@Cc", Convert.ToBase64String(encryptData(tbCc.Text.Trim())));
                            cmd.Parameters.AddWithValue("@CcCVV", Convert.ToBase64String(encryptData(tbCcCVV.Text.Trim())));
                            cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(tbEmail.Text.Trim()));
                            cmd.Parameters.AddWithValue("@DOB", HttpUtility.HtmlEncode(tbDob.Text.Trim()));
                            cmd.Parameters.AddWithValue("@DateTimeRegistered", DateTime.Now);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));

                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    using (SqlCommand cmd2 = new SqlCommand("INSERT INTO Password VALUES(@Email,@PasswordHash,@PasswordSalt,@PH1,@PS1,@PH2,@PS2,@LockStatus,@LockoutTime,@LockoutEndTime,@MinPassAge,@MaxPassAge)"))
                    {
                        using (SqlDataAdapter sda2 = new SqlDataAdapter())
                        {
                            cmd2.CommandType = CommandType.Text;
                            // Now its the password information.
                            cmd2.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(tbEmail.Text.Trim()));
                            cmd2.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd2.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd2.Parameters.AddWithValue("@PH1", finalHash);
                            cmd2.Parameters.AddWithValue("@PS1", salt);
                            cmd2.Parameters.AddWithValue("@PH2", DBNull.Value);
                            cmd2.Parameters.AddWithValue("@PS2", DBNull.Value);
                            cmd2.Parameters.AddWithValue("@LockStatus", "false");
                            cmd2.Parameters.AddWithValue("@LockoutTime", DateTime.Now);
                            cmd2.Parameters.AddWithValue("@LockoutEndTime", DateTime.Now);
                            cmd2.Parameters.AddWithValue("@MinPassAge", DateTime.Now.AddMinutes(5));
                            cmd2.Parameters.AddWithValue("@MaxPassAge", DateTime.Now.AddMinutes(15));

                            cmd2.Connection = con;
                            con.Open();
                            cmd2.ExecuteNonQuery();
                            con.Close();    
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private int checkPwd(string password)
        {
            int score = 0;

            // Score 1 (Very Weak): Password less than 8 characters
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            // Score 2 (Weak): Does not contain lowercase letter(s)
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            // Score 3 (Medium): Does not contain uppercase letter(s)
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            // Score 4 (Strong): Does not contain numeral(s)
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            // Score 5 (Excellent): Does not contain special character(s)
            if (Regex.IsMatch(password, "[^*${}()&%#@!]"))
            {
                score++;
            }

            return score;
        }
        protected byte [] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();

                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
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
                        lbl_gScore.Text = jsonResponse.ToString();

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