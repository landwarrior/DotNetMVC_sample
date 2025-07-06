using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MvcMovie.Controllers;

public class HelloWorldController : Controller
{
    //
    // GET: /HelloWorld/
    public string Index()
    {
        return "This is my default action...";
    }
    //
    // GET: /HelloWorld/Welcome/
    public string Welcome(string name, int numTimes = 1)
    {
        // こっちはデフォルト
        // return "This is the Welcome action method...";

        // return $"Hello {name}, NumTimes is: {numTimes}";
        // こっちはチュートリアルのもの
        // http://localhost:5266/Helloworld/welcome?name=hoge&numtimes=3 みたいに指定する
        return HtmlEncoder.Default.Encode($"Hello {name}, NumTimes is: {numTimes}");
    }

    public string Welcome2(string name, int ID = 1)
    {
        // こっちはチュートリアルをパクってつくったもの
        // http://localhost:5266/Helloworld/welcome2/3?name=hoge みたいに指定する
        return HtmlEncoder.Default.Encode($"Hello {name}, ID: {ID}");
    }

    /// <summary>
    /// Welcome2 メソッドをオーバーロードしてパラメータを二つ受け取るようにできるかと思ったけどできなかったから別メソッドにした
    /// </summary>
    /// <param name="name">クエリパラメータの name を受け取る</param>
    /// <param name="ID">パスパラメータの id を受け取る</param>
    /// <param name="id2">パスパラメータの id2 を受け取る</param>
    /// <returns></returns>
    public string Welcome3(string name, int ID = 1, int id2 = 2)
    {
        // こっちはチュートリアルをパクってつくったもの
        // http://localhost:5266/Helloworld/welcome2/3?name=hoge みたいに指定する
        return HtmlEncoder.Default.Encode($"Hello {name}, ID: {ID}, id2: {id2}");
    }
}
