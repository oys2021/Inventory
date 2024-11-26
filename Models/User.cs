using System;


public class User{

public  int id {get;set;}
public  string username {get;set;}
public  string email {get;set;}
public  string password {get;set;}
public  int role {get;set;}
public bool? isActive { get; set; }


public  string firstname {get;set;}

public  string lastname {get;set;}
public  string phone {get;set;}

public DateTime CreatedAt { get; set; }
public bool? RequirePasswordChange { get; set; }

}

public class Role{
public  int id {get;set;}
public  string rolename {get;set;}
public  string description {get;set;}

}

public class UserRole{
    public int id {get;set;} 
    public int userId {get;set;}                             
    public int roleId {get;set;} 
}