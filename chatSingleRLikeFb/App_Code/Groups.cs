using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Groups
{
   public string GROUP_ID;
   public string GROUP_TITLE;
   public List<User> USERS;
    public Groups(string GROUP_ID, string TITLE)
    {
        this.GROUP_ID = GROUP_ID;
        this.GROUP_TITLE = TITLE;
    }

    public Groups(string GROUP_ID, string TITLE,List<User> USERS)
    {
        this.GROUP_ID = GROUP_ID;
        this.GROUP_TITLE = TITLE;
        this.USERS = USERS;
    }
}
