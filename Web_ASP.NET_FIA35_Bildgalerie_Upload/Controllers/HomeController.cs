using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web_ASP.NET_FIA35_Bildgalerie_Upload.Models;

namespace Web_ASP.NET_FIA35_Bildgalerie_Upload.Controllers
{
    // Durch Eingabe von "ctor" und zweimal Tab wird ein Konstruktor einer Klasse in dieser Klasse erzeugt
    //public ClassNamePlaceholder()
    //{

    //}

    public class HomeController : Controller
    {
        // Liste der erlaubten MimeTypes
        List<string> allowedMimeTypes = new List<string> { "image/png", "image/jpeg", "image/gif", "image/webp" };

        // Hierin befinden sich alle Informationen zur Umgebung des Webprojekts
        // "readonly" bewirkt, dass es keine Änderungen geben kann (außer durch den Konstruktor beim Erzeugen der neuen Instanz am Anfang)
        // "private" müsste nicht extra gesetzt werden, weil alle Methoden per Standard auf "private" gesetzt sind
        private readonly IWebHostEnvironment InfoUeberWebserver;

        public HomeController(IWebHostEnvironment _environment)  // Konstruktor ist "public", weil sonst keine Instanz von außen erzeugt wird
        {
            InfoUeberWebserver = _environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Umgebungsinformationen zur Pfadermittlung der Bilder nutzen
            string wwwPath = this.InfoUeberWebserver.WebRootPath;
            string contentPath = this.InfoUeberWebserver.ContentRootPath;

            DirectoryInfo di = new DirectoryInfo(wwwPath + "\\Bilder");
            FileInfo[] fi = di.GetFiles("*.*");
            List<FileInfo> DateiListe = new List<FileInfo>(fi);

            // Model mit Liste der Dateiinfos
            BilderListe bilderListe = new BilderListe { listDateiInfos = DateiListe };

            return View(bilderListe);
        }

        [HttpPost]
        public IActionResult Index(int id)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(Bildcs neuBild)
        {
            // Prüfen, ob MimeType erlaubt ist:
            if (!allowedMimeTypes.Contains(neuBild.file.ContentType))
            {
                // Wenn die Datei nicht vom einem Mime - Typ der erlaubten Typen ist, wird die View erneut leer aufgerufen
                return View();
            }

            SpeichernBild(neuBild);

            return View();
        }

        private void SpeichernBild(Bildcs neuBild)
        {
            string guid = Guid.NewGuid().ToString();
            string wwwPath = this.InfoUeberWebserver.WebRootPath;
            string contentPath = this.InfoUeberWebserver.ContentRootPath;

            // Falls der Ordner "Bilder" noch nicht vorhanden ist, wird er erzeugt
            string path = Path.Combine(this.InfoUeberWebserver.WebRootPath, "Bilder");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Um Files wieder zu löschen
            // System.IO.File.Delete("/wwwroot...");

            // Speichern / Uploaden
            string neuFName = guid + neuBild.file.FileName.Substring(neuBild.file.FileName.LastIndexOf("."));
            using (FileStream Fstream = new FileStream(wwwPath + "/Bilder/" + neuFName, FileMode.Create))
            {
                neuBild.file.CopyTo(Fstream);
                ViewBag.Dateiname = neuBild.file.FileName;
            }
        }
    }
}
