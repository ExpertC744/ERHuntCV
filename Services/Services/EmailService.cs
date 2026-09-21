using System.Net;
using System.Net.Mail;

namespace ERHuntCV.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // =========================================================
        // COMMON SMTP SETTINGS
        // =========================================================

        private SmtpClient CreateSmtpClient()
        {
            string smtpServer =
                _configuration["EmailSettings:SmtpServer"] ?? "";

            int port =
                Convert.ToInt32(
                    _configuration["EmailSettings:Port"] ?? "587");

            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string smtpPassword =
                _configuration["EmailSettings:Password"] ?? "";

            if (string.IsNullOrWhiteSpace(smtpServer))
                throw new Exception(
                    "EmailSettings:SmtpServer is not configured.");

            if (string.IsNullOrWhiteSpace(username))
                throw new Exception(
                    "EmailSettings:Username is not configured.");

            if (string.IsNullOrWhiteSpace(smtpPassword))
                throw new Exception(
                    "EmailSettings:Password is not configured.");

            SmtpClient smtp =
                new SmtpClient(smtpServer, port);

            smtp.UseDefaultCredentials = false;

            smtp.Credentials =
                new NetworkCredential(
                    username,
                    smtpPassword);

            smtp.EnableSsl = true;

            smtp.DeliveryMethod =
                SmtpDeliveryMethod.Network;

            return smtp;
        }


        // =========================================================
        // COMMON FROM EMAIL
        // =========================================================

        private MailAddress GetFromAddress()
        {
            string username =
                _configuration["EmailSettings:Username"] ?? "";

            string fromEmail =
                _configuration["EmailSettings:FromEmail"];

            string fromName =
                _configuration["EmailSettings:FromName"]
                ?? "Er Job Portal";

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                fromEmail = username;
            }

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new Exception(
                    "EmailSettings:FromEmail is not configured.");
            }

            return new MailAddress(
                fromEmail,
                fromName);
        }


        // =========================================================
        // SUPER ADMIN LOGIN DETAILS
        // =========================================================

        public async Task SendSuperAdminLoginDetailsAsync(
            string email,
            string password)
        {
            try
            {
                using MailMessage message =
                    new MailMessage();

                message.From =
                    GetFromAddress();

                message.To.Add(
                    new MailAddress(email));

                message.Subject =
                    "Your Er Job Portal Super Admin Login Details";

                message.Body = $@"
Dear Super Admin,

You requested your login details for your Er Job Portal Super Admin account.

Your login details are:

Email ID: {email}
Password: {password}

You can use these credentials to login to the Super Admin panel.

Regards,

Er Job Portal Team
";

                message.IsBodyHtml = false;

                using SmtpClient smtp =
                    CreateSmtpClient();

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Super Admin email sending failed: "
                    + ex.Message,
                    ex);
            }
        }


        // =========================================================
        // ORGANIZATION LOGIN DETAILS
        // =========================================================

        public async Task SendOrganizationLoginDetailsAsync(
            string email,
            string password)
        {
            try
            {
                using MailMessage message =
                    new MailMessage();

                message.From =
                    GetFromAddress();

                message.To.Add(
                    new MailAddress(email));

                message.Subject =
                    "Your Er Job Portal Organization Login Details";

                message.Body = $@"
Dear Organization,

You requested your login details for your Er Job Portal organization account.

Your login details are:

Email ID: {email}
Password: {password}

You can use these credentials to login to your organization account.

Regards,

Er Job Portal Team
";

                message.IsBodyHtml = false;

                using SmtpClient smtp =
                    CreateSmtpClient();

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Organization email sending failed: "
                    + ex.Message,
                    ex);
            }
        }


        // =========================================================
        // CANDIDATE LOGIN DETAILS
        // =========================================================

        public async Task SendCandidateLoginDetailsAsync(
            string email,
            string password)
        {
            try
            {
                // ---------------------------------------------
                // Validate recipient
                // ---------------------------------------------

                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new Exception(
                        "Candidate email address is empty.");
                }

                email = email.Trim();

                // ---------------------------------------------
                // Create email
                // ---------------------------------------------

                using MailMessage message =
                    new MailMessage();

                message.From =
                    GetFromAddress();

                message.To.Add(
                    new MailAddress(email));

                message.Subject =
                    "Your Er Job Portal Login Details";

                message.Body = $@"
Dear Candidate,

You requested your login details for your Er Job Portal account.

Your login details are:

Email ID: {email}
Password: {password}

You can use these credentials to login to your candidate account.

Regards,

Er Job Portal Team
";

                message.IsBodyHtml = false;

                // ---------------------------------------------
                // SMTP
                // ---------------------------------------------

                using SmtpClient smtp =
                    CreateSmtpClient();

                await smtp.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                throw new Exception(
                    "SMTP email sending failed. " +
                    "Recipient: " + email +
                    ". SMTP Error: " + ex.Message,
                    ex);
            }
            catch (FormatException ex)
            {
                throw new Exception(
                    "Invalid candidate email address: "
                    + email +
                    ". Error: " + ex.Message,
                    ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Candidate email sending failed. " +
                    "Recipient: " + email +
                    ". Error: " + ex.Message,
                    ex);
            }
        }


        // contacting email

        // =========================================================
        // CONTACT US - THANK YOU EMAIL
        // =========================================================

        // =========================================================
        // CONTACT US - THANK YOU EMAIL
        // =========================================================

        public async Task SendContactThankYouEmailAsync(
            string name,
            string email,
            string subject,
            string description)
        {
            try
            {
                // ---------------------------------------------
                // Validate recipient
                // ---------------------------------------------

                if (string.IsNullOrWhiteSpace(email))
                {
                    throw new Exception(
                        "Contact email address is empty.");
                }

                email = email.Trim();

                name = name?.Trim() ?? "";
                subject = subject?.Trim() ?? "";
                description = description?.Trim() ?? "";

                // ---------------------------------------------
                // Validate email format
                // ---------------------------------------------

                MailAddress recipient;

                try
                {
                    recipient = new MailAddress(email);
                }
                catch (FormatException)
                {
                    throw new Exception(
                        "Invalid contact email address: " + email);
                }

                // ---------------------------------------------
                // Create email
                // ---------------------------------------------

                using MailMessage message = new MailMessage();

                // Your configured Er Job Portal email
                message.From = GetFromAddress();

                // IMPORTANT:
                // Email entered by the user in Contact Us form
                message.To.Add(recipient);

                message.Subject = "Thank You for Contacting Us";

                message.Body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
</head>

<body style='margin:0;
             padding:0;
             background:#f5f7f7;
             font-family:Arial,Helvetica,sans-serif;'>

    <div style='max-width:650px;
                margin:30px auto;
                background:#ffffff;
                border:1px solid #e1e8e7;
                border-radius:10px;
                overflow:hidden;'>

        <!-- HEADER -->

        <div style='background:#0d9488;
                    padding:25px;
                    text-align:center;'>

            <h1 style='margin:0;
                       color:#ffffff;
                       font-size:24px;'>
                Thank You for Contacting Us
            </h1>

        </div>

        <!-- CONTENT -->

        <div style='padding:30px;'>

            <p style='font-size:16px;
                      color:#26333c;'>
                Dear <strong>
                    {System.Net.WebUtility.HtmlEncode(name)}
                </strong>,
            </p>

            <p style='font-size:14px;
                      color:#52616b;
                      line-height:1.7;'>

                Thank you for contacting us.

                We have successfully received your message.
                Our team will review your enquiry and get back
                to you as soon as possible.

            </p>

            <!-- ENQUIRY DETAILS -->

            <div style='margin-top:25px;
                        padding:18px;
                        background:#f3faf9;
                        border:1px solid #e0efed;
                        border-radius:8px;'>

                <h3 style='margin-top:0;
                           color:#0d9488;
                           font-size:17px;'>
                    Your Enquiry Details
                </h3>

                <p style='font-size:14px;
                          color:#26333c;'>

                    <strong>Subject:</strong>
                    {System.Net.WebUtility.HtmlEncode(subject)}

                </p>

                <p style='font-size:14px;
                          color:#26333c;
                          line-height:1.6;'>

                    <strong>Message:</strong><br />

                    {System.Net.WebUtility.HtmlEncode(description)}

                </p>

            </div>

            <p style='font-size:14px;
                      color:#52616b;
                      line-height:1.7;
                      margin-top:25px;'>

                We appreciate your interest in our services.

                If you have any additional information to share,
                please feel free to contact us.

            </p>

            <p style='font-size:14px;
                      color:#26333c;
                      margin-top:25px;'>

                Regards,<br />

                <strong>Er Job Portal Team</strong>

            </p>

        </div>

        <!-- FOOTER -->

        <div style='background:#f6faf9;
                    padding:15px;
                    text-align:center;
                    border-top:1px solid #e5eeee;'>

            <p style='margin:0;
                      font-size:12px;
                      color:#7a858d;'>

                This is an automated acknowledgement email.

            </p>

        </div>

    </div>

</body>
</html>
";

                message.IsBodyHtml = true;

                // ---------------------------------------------
                // SEND EMAIL
                // ---------------------------------------------

                using SmtpClient smtp = CreateSmtpClient();

                await smtp.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                throw new Exception(
                    "SMTP email sending failed. " +
                    "Recipient: " + email +
                    ". SMTP Error: " + ex.Message,
                    ex);
            }
            catch (FormatException ex)
            {
                throw new Exception(
                    "Invalid contact email address: " +
                    email +
                    ". Error: " + ex.Message,
                    ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Contact thank-you email sending failed. " +
                    "Recipient: " + email +
                    ". Error: " + ex.Message,
                    ex);
            }
        }
    }
}