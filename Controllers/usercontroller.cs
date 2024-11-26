using System;
using Microsoft.AspNetCore.Mvc;

namespace ISMS.Controllers{

public class userController:Controller{

private readonly Databasehelper _databaseHelper;
public userController(Databasehelper databasehelper){
_databaseHelper=databasehelper;
}

[HttpGet]
public IActionResult userlogin(){
    return View();
}

[HttpPost]
public IActionResult userlogin(string username,string password){
    // var user = _databaseHelper.getUser(username,"opokuyawsarfo3@gmail.com");
    var user=_databaseHelper.getUserbyUsername(username);
    HttpContext.Session.SetString("UserId", user[0].id.ToString());
    HttpContext.Session.SetString("Username", user[0].username);
     if (user == null || user[0].password != _databaseHelper.HashPassword(password) || !(user[0].isActive ?? false))
        {

            ViewData["Message"] = "Invalid username or password."; 
            Console.WriteLine($"{_databaseHelper.HashPassword(password)},{user[0].password}");
            return View();
        }

    else if((user[0].RequirePasswordChange ?? false)){
        ViewData["Message"] = "Reguire password Change."; 
        Console.WriteLine($"{ViewData["Message"]}");
        return RedirectToAction("changepassword", "user"); 
    }
   
    return RedirectToAction("home", "admin"); 
}



public IActionResult changepassword(){
    return View();
}


[HttpPost]
public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
{
    var userId = HttpContext.Session.GetString("UserId"); 
    Console.WriteLine($"{userId}");
   
    
    var username = HttpContext.Session.GetString("Username");
    Console.WriteLine($"{username}");
   

    var user = _databaseHelper.getUserbyUsername(username);
     Console.WriteLine($"{user[0].password}");
     Console.WriteLine($"{_databaseHelper.HashPassword(oldPassword)}");

    if (user == null)
    {
        ViewData["Message"] = "User not found.";
        Console.WriteLine($"{ViewData["Message"]}");
        return View();
    }

    if (user[0].password != _databaseHelper.HashPassword(oldPassword))
    {
        ViewData["Message"] = "Old password is incorrect.";
       Console.WriteLine($"{ ViewData["Message"]}");
        return View();
    }

    if (newPassword != confirmPassword)
    {
        ViewData["Message"] = "New passwords do not match.";
        Console.WriteLine($"{ViewData["Message"]}");
        return View();
    }
    Console.WriteLine($"this is my id{user[0].id}");
    var updateResult = _databaseHelper.UpdatePassword(user[0].id, _databaseHelper.HashPassword(newPassword));
    Console.WriteLine($"new update {updateResult}");

    if (updateResult)
    {
        ViewData["Message"] = "Password Successfully changed.";
        return RedirectToAction("home", "admin");
    }
    else
    {
        ViewData["Message"] = "Error occurred while updating the password.";
        Console.WriteLine($"{ViewData["Message"] }");
        return View();
    }
}

public IActionResult welcome(){
    return View();
}

}

}