using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.Runtime;

namespace msbuilder.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApplicationEnvironment _host;

        public HomeController(IApplicationEnvironment host)
        {
            _host = host;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }

        public IActionResult Build(string csproj)
        {
            if (string.IsNullOrEmpty(csproj))
            {
                return RedirectToAction("Index");
            }

            var msbuild = @"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe";
            var htmlify = Path.Combine(_host.ApplicationBasePath, "Get-ConsoleAsHtml.ps1");
            var tempHtml = Path.Combine(Path.GetTempPath(), "temp.html");

            if (System.IO.File.Exists(tempHtml))
            {
                System.IO.File.Delete(tempHtml);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $@"-NoProfile -ExecutionPolicy unrestricted -Command ""& {{& '{msbuild}' '{csproj}'}}; {htmlify} | out-file '{tempHtml}' -encoding UTF8""",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process {StartInfo = startInfo};
            process.Start();
            process.WaitForExit();

            var outputHtml = System.IO.File.ReadAllText(tempHtml);
            ViewBag.BuildOutput = new HtmlString(outputHtml);
            ViewBag.Csproj = csproj;

            return View();
        }
    }
}
