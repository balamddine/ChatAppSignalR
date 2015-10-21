using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Friends
/// </summary>
public class User
{
	   public string USER_ID { get; set; }
       public string USER_CONTEXT { get; set; }
       public string USER_USERNAME { get; set; }
       public string USER_EMAIL { get; set; }
       public string USER_LOGO { get; set; }
       public string USER_MOOD { get; set; }
       public string USER_STATUS_ID { get; set; }
       public string STATUS_DESCRIPTION { get; set; }
       public User(string USER_ID, string USER_USERNAME,string USER_EMAIL, string USER_LOGO, string USER_MOOD, string USER_STATUS_ID, string STATUS_DESCRIPTION)
       {
           this.USER_ID = USER_ID;
           this.USER_USERNAME = USER_USERNAME;
           this.USER_EMAIL = USER_EMAIL;
           this.USER_LOGO = USER_LOGO;
           this.USER_MOOD = USER_MOOD;
           this.USER_STATUS_ID = USER_STATUS_ID;
           this.STATUS_DESCRIPTION = STATUS_DESCRIPTION;
       }

       public User(string USER_CONTEXT)
       {
           this.USER_CONTEXT = USER_CONTEXT;
       }

       public User(){}

       public User(string USER_ID,bool isID) {
           this.USER_ID = USER_ID;
       }
}