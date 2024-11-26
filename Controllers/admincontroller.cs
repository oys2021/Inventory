using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace ISMS.Controllers{

public class admincontroller:Controller{
private readonly Databasehelper _databaseHelper;
public admincontroller(Databasehelper databasehelper){
_databaseHelper=databasehelper;
}

public IActionResult home(){
    return View();
}

[HttpGet]
public IActionResult adduser(){
    var result=_databaseHelper.getRole();
    ViewData["roles"]=result;
    return View();
} 
[HttpPost]
public IActionResult adduser(string firstname, string lastname, string username, string phone, string email, int role)
{
    var existingUser = _databaseHelper.getUser(username, email);
    if (existingUser != null)
    {
        var result = _databaseHelper.getRole();
        ViewData["roles"] = result;
        // Console.WriteLine($"User already exists");
        return View();
    }
    else
    {
        Console.WriteLine($"Creating new user - Role: {role}");
        var result = _databaseHelper.getRole();
        ViewData["roles"] = result;

      
        var newresult = _databaseHelper.createuser(username, email, role, firstname, lastname, phone);

        var newUser = _databaseHelper.getUser(username, email);

        return RedirectToAction("userlogin","user"); 
        // newlogin

    }
}


[HttpGet]
public IActionResult addrole(){
    var result=_databaseHelper.getRole();
    foreach(var role in result){
        Console.WriteLine(role.id);
        Console.WriteLine(role.rolename);
        Console.WriteLine(role.description);
    }
    return View();
} 

[HttpPost]
public IActionResult addrole(string rolename,string description){
var result=_databaseHelper.createrole(rolename,description);
if (result){
    ViewData["Message"]="Role Added Succesfully";
    Console.WriteLine($"{ViewData["Message"]}");
    return View();
}
else{
    ViewData["Message"]="Role Addition  failed";
    Console.WriteLine($"{ViewData["Message"]}");
    return View();
}
}

}

}