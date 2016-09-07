using UnityEngine;
using System.Collections;

public interface ILoginable
{
    void Login(string username, string password);

    void Logout();
}
