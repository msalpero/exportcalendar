# exportcalendar
a c# application for export Exchange user calendar information via EWS

Application account must have impersonation permission . If you do not provide any custom account during run it will use currently logged user account .

https://docs.microsoft.com/en-us/exchange/client-developer/exchange-web-services/how-to-configure-impersonation

CSV file must be single column (email address) like ;

MBX
user1@domain.com
user2@domain.com
user3@domain.com
user4@domain.com

Or you can get single mailbox calendar information (press set after type the user account) . 

It will create an error log in the folder of application.

Output folder c:\output


